using Rayer.Core;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Models.Response.Search;
using Rayer.SearchEngine.ViewModels;
using Rayer.SearchEngine.ViewModels.Presenter;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Rayer.SearchEngine.Views.Pages;

[Inject(ServiceLifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped)]
public partial class SearchPage : INavigableView<SearchViewModel>, INavigationAware
{
    private readonly ISearchPresenterProvider _searchPresenterProvider;

    public SearchPage()
    {
        _searchPresenterProvider = AppCore.GetRequiredService<ISearchPresenterProvider>();

        var vm = AppCore.GetRequiredService<SearchViewModel>();

        ViewModel = vm;

        InitializeComponent();
    }

    public SearchViewModel ViewModel { get; set; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is SearchAggregationModel model)
        {
            ViewModel.Model = model;

            DataContext = this;
        }
    }

    private async void OnCheckedChanged(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is RadioButton radioButton)
        {
            await SearchProcess(radioButton);
        }
    }

    private async Task SearchProcess(RadioButton radio)
    {
        Loading.Visibility = Visibility.Visible;

        if (radio.Content is System.Windows.Controls.TextBlock textBlock)
        {
            if (textBlock.Text is "歌曲")
            {
                var dataContext = await ViewModel.LoadAudioAsync();

                ApplyPresenter<SearchAudioPresenterViewModel, SearchAudioDetailResponse>(textBlock.Text, dataContext);

                Loading.Visibility = Visibility.Collapsed;
            }
            else if (textBlock.Text is "艺人")
            {
                await ViewModel.LoadSingerAsync();
            }
            else if (textBlock.Text is "专辑")
            {
                await ViewModel.LoadAlbumAsync();
            }
            else if (textBlock.Text is "视频")
            {
                await ViewModel.LoadVideoAsync();
            }
            else if (textBlock.Text is "歌单")
            {
                await ViewModel.LoadPlaylistAsync();
            }
        }
    }

    private void ApplyPresenter<TViewModel, TResponse>(string identifier, TResponse dataContext)
        where TViewModel : class, IPresenterViewModel<TResponse>
        where TResponse : class
    {
        var presenter = _searchPresenterProvider.GetPresenter<TViewModel, TResponse>(identifier);

        if (presenter is UserControl control)
        {
            if (Presenter.Children.Count > 0)
            {
                Presenter.Children.Remove(Presenter.Children[0]);
            }

            Presenter.Children.Add(control);

            control.Width = ActualWidth;
            control.Height = ActualHeight;

            presenter.ViewModel.PresenterDataContext = dataContext;
        }
    }

    public void OnNavigatedTo()
    {
        var titleBar = AppCore.GetRequiredService<SearchTitleBar>();
        titleBar.CheckedChanged += OnCheckedChanged;

        if (DataContext is SearchAggregationModel model)
        {
            ViewModel.Model = model;

            DataContext = this;

            titleBar.DefaultPage.IsChecked = true;
        }

        var navView = AppCore.GetRequiredService<INavigationService>().GetNavigationControl() as NavigationView;

        if (navView?.Template.FindName("PART_NavigationViewContentPresenter", navView) is NavigationViewContentPresenter navPresenter)
        {
            ScrollViewer.SetCanContentScroll(navPresenter, false);
            ScrollViewer.SetHorizontalScrollBarVisibility(navPresenter, ScrollBarVisibility.Disabled);
            ScrollViewer.SetVerticalScrollBarVisibility(navPresenter, ScrollBarVisibility.Disabled);
            ScrollViewer.SetIsDeferredScrollingEnabled(navPresenter, false);
        }

        var navigationHeaderUpdater = AppCore.GetRequiredService<INavigationHeaderUpdater>();
        navigationHeaderUpdater.Show(titleBar);

        AppCore.MainWindow.SizeChanged += OnWindowSizeChanged;
    }

    public void OnNavigatedFrom()
    {
        var navigationHeaderUpdater = AppCore.GetRequiredService<INavigationHeaderUpdater>();

        navigationHeaderUpdater.Hide();

        var titleBar = AppCore.GetRequiredService<SearchTitleBar>();
        titleBar.CheckedChanged -= OnCheckedChanged;

        AppCore.MainWindow.SizeChanged -= OnWindowSizeChanged;
    }

    private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (Presenter.Children.Count > 0 && Presenter.Children[0] is FrameworkElement element)
        {
            element.Width = e.NewSize.Width - 230;
            element.Height = e.NewSize.Height;
        }
    }
}