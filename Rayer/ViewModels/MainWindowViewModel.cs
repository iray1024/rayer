using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Views.Pages;
using Rayer.Views.Pages;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Rayer.ViewModels;

[Inject]
public partial class MainWindowViewModel : ObservableObject
{
    private readonly ISearchEngine _searchEngine;
    private readonly INavigationService _navigationService;

    public MainWindowViewModel(
        ISearchEngine searchEngine,
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

        _searchEngine = searchEngine;
        _navigationService = navigationService;
    }

    [ObservableProperty]
    private string _applicationTitle = "Rayer-Music";

    [ObservableProperty]
    private ICollection<object> _menuItems =
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

    public async Task OnAutoSuggestTextChanged(AutoSuggestBoxTextChangedEventArgs args)
    {

    }

    public async Task OnAutoSuggestQuerySubmitted(AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var model = await _searchEngine.SearchAsync(args.QueryText, AppCore.StoppingToken);

        if (_navigationService.GetNavigationControl().SelectedItem?.TargetPageType != typeof(SearchPage))
        {
            _navigationService.Navigate(typeof(SearchPage), model);
        }
        else
        {
            var searchAware = App.GetRequiredService<SearchPage>();

            await searchAware.OnSearchAsync(model);
        }
    }

    public async Task OnAutoSuggestChosen(AutoSuggestBoxSuggestionChosenEventArgs args)
    {

    }
}