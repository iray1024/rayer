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

    private string _currentSuggestText = string.Empty;
    private bool _userRaiseClickSuggestItem = false;

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

    #region Properties    
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

                    var model = await _searchEngine.SuggestAsync(args.Text);

                    if (model is not null && model.Code == 200)
                    {
                        box.ItemsSource = model.Result.Audios.Length > 0
                            ? model.Result.Audios.Select(x => x.Name).ToList()
                            : null;
                    }
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

        var model = await _searchEngine.SearchAsync(args.QueryText, AppCore.StoppingToken);

        if (_navigationService.GetNavigationControl().SelectedItem?.TargetPageType != typeof(SearchPage))
        {
            _navigationService.Navigate(typeof(SearchPage), model);
        }

        var searchAware = App.GetRequiredService<SearchPage>();

        searchAware.OnSearch(model);
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

            var model = await _searchEngine.SearchAsync(_currentSuggestText, AppCore.StoppingToken);

            _ = Interlocked.Exchange(ref _currentSuggestText, string.Empty);

            if (_navigationService.GetNavigationControl().SelectedItem?.TargetPageType != typeof(SearchPage))
            {
                _navigationService.Navigate(typeof(SearchPage), model);
            }

            var searchAware = App.GetRequiredService<SearchPage>();

            searchAware.OnSearch(model);
        }
    }
}