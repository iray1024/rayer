﻿using Microsoft.Extensions.Options;
using Rayer.Abstractions;
using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Menu;
using Rayer.Core.PlayControl.Abstractions;
using Rayer.Core.Playing;
using Rayer.Core.Utils;
using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Options;
using Rayer.SearchEngine.Views.Windows;
using Rayer.Services;
using Rayer.ViewModels;
using Rayer.Views.Pages;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Rayer;

[Inject<IWindow>(ResolveServiceType = true)]
public partial class MainWindow : IWindow
{
    private bool _isUserClosedPane;
    private bool _isPaneOpenedOrClosedFromCode;

    private readonly SystemMediaTransportControlsManager _smtc = new();

    public MainWindow(
        MainWindowViewModel viewModel,
        IServiceProvider serviceProvider,
        INavigationService navigationService,
        ISnackbarService snackbarService,
        IContentDialogService contentDialogService,
        ILoaderProvider loaderProvider)
    {
        SystemThemeWatcher.Watch(this);

        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        var settings = App.GetRequiredService<ISettingsService>();
        ApplicationThemeManager.Apply(settings.Settings.Theme, WindowBackdropType.Mica, true);

        navigationService.SetNavigationControl(NavigationView);
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        contentDialogService.SetDialogHost(RootContentDialog);
        loaderProvider.SetLoader(Loader, 160, 0);

        NavigationView.SetServiceProvider(serviceProvider);

        ApplicationThemeManager.Changed += OnThemeChanged;

        RenderOptions.ProcessRenderMode = RenderMode.Default;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        ApplyNavigationMenuIcons();

        var immersivePlayerService = App.GetRequiredService<IImmersivePlayerService>();

        immersivePlayerService.SetPlayer(ImmersivePlayer);

        var dynamicIsland = App.GetRequiredService<DynamicIsland>();
        dynamicIsland.Show();

        AutoSuggest.TextChanged += OnAutoSuggestTextChanged;
        AutoSuggest.SuggestionChosen += OnSuggestionChosen;
        AutoSuggest.QuerySubmitted += OnAutoSuggestQuerySubmitted;

        await OnBootloaderInjectingAsync();

        InitializeTaskbarInfo();

        var audioManager = AppCore.GetRequiredService<IAudioManager>();
        var playbar = AppCore.GetRequiredService<IPlaybarService>();
        var windowHandle = new WindowInteropHelper(this).Handle;
        _smtc.Initialize(
            windowHandle,
            () => Application.Current.Dispatcher.Invoke(() => playbar.PlayOrPause()),
            () => Application.Current.Dispatcher.Invoke(() => playbar.PlayOrPause()),
            () => Application.Current.Dispatcher.BeginInvoke(async () => await playbar.Next()),
            () => Application.Current.Dispatcher.BeginInvoke(async () => await playbar.Previous()));

        audioManager.AudioChanged += async (s, e) =>
        {
            await _smtc.UpdateMetadata(e.New);
        };

        audioManager.AudioPlaying += (s, e) =>
        {
            _smtc.UpdatePlaybackStatus(Windows.Media.MediaPlaybackStatus.Playing);
        };

        audioManager.AudioPaused += (s, e) =>
        {
            _smtc.UpdatePlaybackStatus(Windows.Media.MediaPlaybackStatus.Paused);
        };

        audioManager.AudioStopped += (s, e) =>
        {
            _smtc.UpdatePlaybackStatus(Windows.Media.MediaPlaybackStatus.Stopped);
        };

        audioManager.Playback.Seeked += (s, e) =>
        {
            _smtc.UpdateSeek(audioManager.Playback);
        };

        audioManager.PreLoaded += async (s, e) =>
        {
            await Application.Current.Dispatcher.InvokeAsync(() => audioManager.Playback.RecoveryAsync());
        };

#if RELEASE
        var updater = AppCore.GetRequiredService<IUpdateService>();
        var (checkResult, _) = await updater.CheckUpdateAsync(AppCore.StoppingToken);
        if (checkResult == true)
        {
            await updater.UpdateAsync(AppCore.StoppingToken);
        }
#endif
    }

    public MainWindowViewModel ViewModel { get; set; } = null!;

    private void OnNavigating(NavigationView sender, NavigatingCancelEventArgs args)
    {
        var pageType = args.Page.GetType();

        NavigationView.HeaderVisibility =
            //pageType != typeof(AudioLibraryPage) &&
            pageType != typeof(SettingsPage) &&
            pageType != typeof(PlaylistPage)
                ? Visibility.Collapsed
                : Visibility.Visible;

        PageHeaderContainer.Visibility =
            //pageType != typeof(AudioLibraryPage) &&
            pageType != typeof(SettingsPage) &&
            pageType != typeof(PlaylistPage)
                ? Visibility.Collapsed
                : Visibility.Visible;

        if (pageType == typeof(PlaylistPage))
        {
            ProcessPlaylistUpdateWithoutNavigated();
        }
    }

    private void OnNavigationSelectionChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not NavigationView navigationView)
        {
            return;
        }

        PageHeader.Text = navigationView.SelectedItem?.Content.ToString();

        ProcessPlaylistNavigation(navigationView);
    }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        ApplyNavigationMenuIcons();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_isUserClosedPane)
        {
            return;
        }

        _isPaneOpenedOrClosedFromCode = true;
        NavigationView.IsPaneOpen = !(e.NewSize.Width <= 1000);
        _isPaneOpenedOrClosedFromCode = false;
    }

    private void NavigationView_OnPaneOpened(NavigationView sender, RoutedEventArgs args)
    {
        if (_isPaneOpenedOrClosedFromCode)
        {
            return;
        }

        _isUserClosedPane = false;
    }

    private void NavigationView_OnPaneClosed(NavigationView sender, RoutedEventArgs args)
    {
        if (_isPaneOpenedOrClosedFromCode)
        {
            return;
        }

        _isUserClosedPane = true;
    }

    private async void OnAutoSuggestTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        await ViewModel.OnAutoSuggestTextChanged(args);
    }

    private async void OnAutoSuggestQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.OnAutoSuggestQuerySubmitted(args);
    }

    private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        ViewModel.OnAutoSuggestChosen(args);
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void ApplyNavigationMenuIcons()
    {
        foreach (var item in NavigationView.MenuItems)
        {
            if (item is NavigationViewItem vItem)
            {
                var iconSource = (ImageSource)Application.Current.Resources[vItem.TargetPageTag];

                if (iconSource is not null)
                {
                    vItem.Icon = new ImageIcon()
                    {
                        Source = iconSource,
                        Width = 24,
                        Height = 24,
                    };
                }
            }
        }
    }

    private static readonly Action _toggleProcess = () => App.GetRequiredService<ProcessMessageWindow>().ToggleProcess();

    private void OnAutoSuggestGotFocus(object sender, RoutedEventArgs e)
    {
        _toggleProcess();
    }

    private void OnAutoSuggestLostFocus(object sender, RoutedEventArgs e)
    {
        _toggleProcess();
    }

    private async void OnAutoSuggestPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (AutoSuggest.IsSuggestionListOpen &&
            e.Key is Key.Enter)
        {
            await ViewModel.OnUserRaiseAutoSuggestChosen(AutoSuggest);
        }
    }

    private static async Task OnBootloaderInjectingAsync()
    {
        var snackbar = App.GetRequiredService<ISnackbarFactory>();
        var bootloader = App.GetRequiredService<IIPSBootloader>();

#if DEBUG
        var logger = LocalLoggerFactory.GetOrCreateLogger(AppCore.ServiceProvider, "Rayer Server");
        var uri = await bootloader.RunAsync(logger);
#else
        var uri = await bootloader.RunAsync();
#endif
        var searchEngineOptions = App.GetRequiredService<IOptionsSnapshot<SearchEngineOptions>>().Value;

        searchEngineOptions.HttpEndpoint = uri.OriginalString;

        snackbar.ShowSecondary(
            "Cloud Server",
            $"Cloud Server注入成功",
            TimeSpan.FromSeconds(3));
    }

    private void InitializeTaskbarInfo()
    {
        var commandBinding = App.GetRequiredService<ICommandBinding>();

        TaskbarItemInfo = new TaskbarItemInfo
        {
            Description = "喵蛙王子丶的音乐播放器"
        };

        var previous = new ThumbButtonInfo
        {
            Command = commandBinding.PreviousCommand,
            ImageSource = ImageSourceFactory.Create("pack://application:,,,/assets/dark/previous_24x24.png"),
            Description = "上一首",
            IsEnabled = false,
        };

        var playOrPause = new ThumbButtonInfo
        {
            Command = commandBinding.PlayOrPauseCommand,
            ImageSource = ImageSourceFactory.Create("pack://application:,,,/assets/dark/play_24x24.png"),
            Description = "播放",
            IsEnabled = false
        };

        var next = new ThumbButtonInfo
        {
            Command = commandBinding.NextCommand,
            ImageSource = ImageSourceFactory.Create("pack://application:,,,/assets/dark/next_24x24.png"),
            Description = "下一首",
            IsEnabled = false
        };

        RenderOptions.SetBitmapScalingMode(TaskbarItemInfo, BitmapScalingMode.Fant);
        RenderOptions.SetBitmapScalingMode(previous, BitmapScalingMode.Fant);
        RenderOptions.SetBitmapScalingMode(playOrPause, BitmapScalingMode.Fant);
        RenderOptions.SetBitmapScalingMode(next, BitmapScalingMode.Fant);
        RenderOptions.SetBitmapScalingMode(previous.ImageSource, BitmapScalingMode.Fant);
        RenderOptions.SetBitmapScalingMode(playOrPause.ImageSource, BitmapScalingMode.Fant);
        RenderOptions.SetBitmapScalingMode(next.ImageSource, BitmapScalingMode.Fant);

        TaskbarItemInfo.ThumbButtonInfos.Add(previous);
        TaskbarItemInfo.ThumbButtonInfos.Add(playOrPause);
        TaskbarItemInfo.ThumbButtonInfos.Add(next);
    }

    private static void ProcessPlaylistNavigation(NavigationView navigationView)
    {
        if (navigationView.SelectedItem?.TargetPageType == typeof(PlaylistPage))
        {
            var playlistService = App.GetRequiredService<IPlaylistService>();

            var tag = navigationView.SelectedItem.TargetPageTag;

            if (tag.StartsWith("_playlist_"))
            {
                var page = App.GetRequiredService<PlaylistPage>();
                var id = long.Parse(tag[10..]);

                var model = playlistService.Playlists.FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    page.ViewModel.Items = new Core.Common.SortableObservableCollection<Audio>(model.Audios, AudioSortComparer.Ascending);
                    page.ViewModel.Id = model.Id;
                    page.ViewModel.Name = model.Name;
                }
            }
        }
    }

    private static void ProcessPlaylistUpdateWithoutNavigated()
    {
        var playlistService = App.GetRequiredService<IPlaylistService>();
        var page = App.GetRequiredService<PlaylistPage>();

        var playlist = playlistService.Playlists.FirstOrDefault(x => x.Id == page.ViewModel.Id);

        if (playlist is not null)
        {
            page.ViewModel.Items = new Core.Common.SortableObservableCollection<Audio>(playlist.Audios, AudioSortComparer.Ascending);
        }
    }
}