using Microsoft.Extensions.Options;
using Rayer.Abstractions;
using Rayer.Core;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Menu;
using Rayer.Core.Utils;
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

[Inject<IWindow>]
public partial class MainWindow : IWindow
{
    private bool _isUserClosedPane;
    private bool _isPaneOpenedOrClosedFromCode;

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

    public MainWindowViewModel ViewModel { get; set; } = null!;

    private void OnNavigating(NavigationView sender, NavigatingCancelEventArgs args)
    {
        var pageType = args.Page.GetType();

        NavigationView.HeaderVisibility =
            pageType != typeof(AudioLibraryPage) &&
            pageType != typeof(SettingsPage)
                ? Visibility.Collapsed
                : Visibility.Visible;

        PageHeaderContainer.Visibility =
            pageType != typeof(AudioLibraryPage) &&
            pageType != typeof(SettingsPage)
                ? Visibility.Collapsed
                : Visibility.Visible;
    }

    private void OnNavigationSelectionChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not NavigationView navigationView)
        {
            return;
        }

        PageHeader.Text = navigationView.SelectedItem?.Content.ToString();
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
        foreach (var item in NavigationView.MenuItems.Cast<NavigationViewItem>())
        {
            var iconSource = (ImageSource)Application.Current.Resources[item.TargetPageTag];

            if (iconSource is not null)
            {
                item.Icon = new ImageIcon()
                {
                    Source = iconSource,
                    Width = 24,
                    Height = 24,
                };
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
            ImageSource = ImageSourceUtils.Create("pack://application:,,,/assets/dark/previous.png"),
            Description = "上一首",
            IsEnabled = false,
        };

        var playOrPause = new ThumbButtonInfo
        {
            Command = commandBinding.PlayOrPauseCommand,
            ImageSource = ImageSourceUtils.Create("pack://application:,,,/assets/dark/play.png"),
            Description = "播放",
            IsEnabled = false
        };

        var next = new ThumbButtonInfo
        {
            Command = commandBinding.NextCommand,
            ImageSource = ImageSourceUtils.Create("pack://application:,,,/assets/dark/next.png"),
            Description = "下一首",
            IsEnabled = false
        };

        RenderOptions.SetBitmapScalingMode(TaskbarItemInfo, BitmapScalingMode.HighQuality);
        RenderOptions.SetBitmapScalingMode(previous, BitmapScalingMode.HighQuality);
        RenderOptions.SetBitmapScalingMode(playOrPause, BitmapScalingMode.HighQuality);
        RenderOptions.SetBitmapScalingMode(next, BitmapScalingMode.HighQuality);
        RenderOptions.SetBitmapScalingMode(previous.ImageSource, BitmapScalingMode.HighQuality);
        RenderOptions.SetBitmapScalingMode(playOrPause.ImageSource, BitmapScalingMode.HighQuality);
        RenderOptions.SetBitmapScalingMode(next.ImageSource, BitmapScalingMode.HighQuality);

        TaskbarItemInfo.ThumbButtonInfos.Add(previous);
        TaskbarItemInfo.ThumbButtonInfos.Add(playOrPause);
        TaskbarItemInfo.ThumbButtonInfos.Add(next);
    }
}