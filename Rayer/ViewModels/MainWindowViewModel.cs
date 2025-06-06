using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Framework;
using Rayer.Core.Menu;
using Rayer.Core.Utils;
using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Enums;
using Rayer.SearchEngine.Views.Pages;
using Rayer.Views.Pages;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Controls;
using MenuItem = Wpf.Ui.Controls.MenuItem;

namespace Rayer.ViewModels;

[Inject]
public partial class MainWindowViewModel : ObservableObject
{
    private readonly ILoaderProvider _loaderProvider;
    private readonly ISearchEngineProvider _searchEngineProvider;
    private readonly INavigationService _navigationService;

    private string _currentSuggestText = string.Empty;
    private bool _userRaiseClickSuggestItem = false;
    private readonly string[] _defaultEmptySuggest = ["当前引擎暂无建议"];

    private static readonly RectangleGeometry _defaultClipSetting = new(new Rect(0, 0, 24, 24), 4, 4);

    public MainWindowViewModel(
            INavigationService navigationService)
    {
        var plugins = App.GetServices<INavigationMenuPlugin>();

        foreach (var plugin in plugins)
        {
            foreach (var item in plugin.MenuItems)
            {
                MenuItems.Add(item);
            }
        }

        ProcessPlaylistMenu();

        _loaderProvider = App.GetRequiredService<ILoaderProvider>();
        _searchEngineProvider = App.GetRequiredService<ISearchEngineProvider>();
        _navigationService = navigationService;
    }

    #region Properties    
    [ObservableProperty]
    private string _applicationTitle = "Rayer-Music";

    [ObservableProperty]
    private ObservableCollection<object> _menuItems =
    [
        new NavigationViewItem("本地音乐", SymbolRegular.Home24, typeof(AudioLibraryPage)),
    ];

    [ObservableProperty]
    private ICollection<object> _footerMenuItems =
    [
        new NavigationViewItem("设置", SymbolRegular.Settings24, typeof(SettingsPage))
    ];

    [ObservableProperty]
    private ObservableCollection<MenuItem> _trayMenuItems =
    [
        new MenuItem { Header = "本地音乐", Tag = "tray_audioLibrary" },
        new MenuItem { Header = "退出", Tag = "tray_exit" }
    ];
    #endregion

    public async Task OnAutoSuggestTextChanged(AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Source is AutoSuggestBox box)
        {
            if (args.Reason is AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (!string.IsNullOrEmpty(args.Text))
                {
                    args.Handled = true;

                    var model = await Task.Run(() =>
                        _searchEngineProvider.GetSearchEngine(SearcherType.Netease).SuggestAsync(args.Text, AppCore.StoppingToken),
                        AppCore.StoppingToken);

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        box.ItemsSource = model is not null && model.Code == 200
                            ? model.Audios.Length > 0
                                ? model.Audios.Select(x => x.Name).ToList()
                                : null
                            : _defaultEmptySuggest;
                    });
                }
                else
                {
                    box.ItemsSource = MenuItems.Cast<NavigationViewItem>().Select(x => x.Name);
                }
            }
            else if (args.Reason is AutoSuggestionBoxTextChangeReason.ProgrammaticChange && _userRaiseClickSuggestItem)
            {
                _userRaiseClickSuggestItem = false;
                _ = Interlocked.Exchange(ref _currentSuggestText, args.Text);

                await OnUserRaiseAutoSuggestChosen(box);
            }
        }
    }

    public async Task OnAutoSuggestQuerySubmitted(AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        args.Handled = true;

        _loaderProvider.Loading();

        var model = await Task.Run(() =>
            _searchEngineProvider.SearchEngine.SearchAsync(args.QueryText, SearchType.Audio, AppCore.StoppingToken),
            AppCore.StoppingToken);

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _loaderProvider.Loaded();

            model.QueryText = args.QueryText;

            if (_navigationService.GetNavigationControl().SelectedItem?.TargetPageType != typeof(SearchPage))
            {
                _navigationService.Navigate(typeof(SearchPage), model);
            }

            var searchAware = App.GetRequiredService<ISearchAware>();

            searchAware.OnSearch(model);
        });
    }

    public void OnAutoSuggestChosen(AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.Source is AutoSuggestBox box)
        {
            if (box is { Text.Length: > 0, IsSuggestionListOpen: true } &&
            args.SelectedItem is string { Length: > 0 } queryText)
            {
                args.Handled = true;

                _ = Interlocked.Exchange(ref _currentSuggestText, queryText);
            }
            else
            {
                _userRaiseClickSuggestItem = true;
            }
        }
    }

    public async Task OnUserRaiseAutoSuggestChosen(AutoSuggestBox source)
    {
        if (!string.IsNullOrEmpty(_currentSuggestText))
        {
            source.Text = _currentSuggestText;

            _loaderProvider.Loading();

            var model = await Task.Run(() =>
                _searchEngineProvider.SearchEngine.SearchAsync(_currentSuggestText, SearchType.Audio, AppCore.StoppingToken),
                AppCore.StoppingToken);

            Application.Current.Dispatcher.Invoke(_loaderProvider.Loaded);

            model.QueryText = _currentSuggestText;
            _ = Interlocked.Exchange(ref _currentSuggestText, string.Empty);

            if (_navigationService.GetNavigationControl().SelectedItem?.TargetPageType != typeof(SearchPage))
            {
                _navigationService.Navigate(typeof(SearchPage), model);
            }

            var searchAware = App.GetRequiredService<ISearchAware>();

            searchAware.OnSearch(model);
        }
    }

    private void ProcessPlaylistMenu()
    {
        var commandBindings = App.GetRequiredService<ICommandBinding>();
        var playlistProvider = App.GetRequiredService<IPlaylistProvider>();

        playlistProvider.Initialize(App.GetRequiredService<IAudioManager>());

        var newPlaylistMenuItem = new NavigationViewItem()
        {
            Name = "新建歌单",
            IsMenuElement = false,
            Content = "新建歌单+",
            TargetPageTag = "Playlist",
            Command = commandBindings.AddPlaylistCommand
        };

        MenuItems.Add(newPlaylistMenuItem);

        foreach (var item in playlistProvider.Playlists.OrderBy(x => x.Sort))
        {
            var navViewItem = new NavigationViewItem(item.Name, typeof(PlaylistPage))
            {
                TargetPageTag = $"_playlist_{item.Id}",
            };

            var coverItem = item.Audios.FirstOrDefault();

            if (coverItem is not null)
            {
                if (coverItem.IsVirualWebSource)
                {
                    var cover = !string.IsNullOrEmpty(coverItem.CoverUri)
                        ? ImageSourceFactory.CreateWebSource(coverItem.CoverUri)
                        : (ImageSource)Application.Current.Resources["AlbumFallback"];

                    navViewItem.Icon = new ImageIcon()
                    {
                        Source = cover,
                        Width = 24,
                        Height = 24,
                    };
                }
                else
                {
                    navViewItem.Icon = new ImageIcon()
                    {
                        Source = coverItem.Cover,
                        Width = 24,
                        Height = 24
                    };
                }

                if (navViewItem.Icon is not null)
                {
                    navViewItem.Icon.Clip = _defaultClipSetting;
                }
            }

            MenuItems.Add(navViewItem);
        }
    }
}