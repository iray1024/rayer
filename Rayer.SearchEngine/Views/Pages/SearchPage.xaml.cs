using Rayer.Core;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Utils;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Enums;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Models.Response.Search;
using Rayer.SearchEngine.ViewModels;
using Rayer.SearchEngine.ViewModels.Presenter;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Rayer.SearchEngine.Views.Pages;

[Inject(ServiceLifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
public partial class SearchPage : INavigableView<SearchViewModel>, INavigationAware, ISearchAware
{
    private readonly ISearchPresenterProvider _searchPresenterProvider;
    private readonly ILoaderProvider _loaderProvider;

    private CancellationTokenSource _requestToken = new();

    private int _hasNavigationTo = 0;

    public SearchPage()
    {
        _searchPresenterProvider = AppCore.GetRequiredService<ISearchPresenterProvider>();
        _loaderProvider = AppCore.GetRequiredService<ILoaderProvider>();

        var vm = AppCore.GetRequiredService<SearchViewModel>();

        ViewModel = vm;

        InitializeComponent();
    }

    public bool HasNavigationTo
    {
        get => _hasNavigationTo == 1;
        set
        {
            _ = Interlocked.Exchange(ref _hasNavigationTo, value ? 1 : 0);
        }
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
        if (e.OriginalSource is RadioButton radioButton && radioButton.IsChecked == true)
        {
            _loaderProvider.Loading();

            await _requestToken.CancelAsync();
            _requestToken = new CancellationTokenSource();

            var dispatcherTask = Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await SearchProcess(radioButton, _requestToken.Token);

                _loaderProvider.Loaded();
            },
            DispatcherPriority.Normal,
            _requestToken.Token);
        }
    }

    private async Task SearchProcess(RadioButton radio, CancellationToken cancellationToken = default)
    {
        if (radio.Content is System.Windows.Controls.TextBlock textBlock)
        {
            var searchType = EnumHelper.ParseEnum<SearchType>(textBlock.Text);

            if (searchType is SearchType.Audio)
            {
                var dataContext = await ViewModel.LoadAudioAsync();

                ApplyPresenter<SearchAudioPresenterViewModel, SearchAudioDetailResponse>(searchType, dataContext);
            }
            else if (searchType is SearchType.Singer)
            {
                var dataContext = await ViewModel.LoadSingerAsync();

                await Task.Delay(1000, cancellationToken);

                ApplyPresenter<SearchSingerPresenterViewModel, SearchSingerDetailResponse>(searchType, dataContext);
            }
            else if (searchType is SearchType.Album)
            {
                var dataContext = await ViewModel.LoadAlbumAsync();

                ApplyPresenter<SearchAlbumPresenterViewModel, SearchAlbumDetailResponse>(searchType, dataContext);
            }
            else if (searchType is SearchType.Video)
            {
                var dataContext = await ViewModel.LoadVideoAsync();

                ApplyPresenter<SearchVideoPresenterViewModel, SearchVideoDetailResponse>(searchType, dataContext);
            }
            else if (searchType is SearchType.Playlist)
            {
                var dataContext = await ViewModel.LoadPlaylistAsync();

                ApplyPresenter<SearchPlaylistPresenterViewModel, SearchPlaylistDetailResponse>(searchType, dataContext);
            }
        }
    }

    private void ApplyPresenter<TViewModel, TResponse>(SearchType searchType, TResponse dataContext)
        where TViewModel : class, IPresenterViewModel<TResponse>
        where TResponse : class
    {
        var presenter = _searchPresenterProvider.GetPresenter<TViewModel, TResponse>(searchType);

        if (presenter is UserControl control)
        {
            if (Presenter.Children.Count > 0)
            {
                Presenter.Children.Remove(Presenter.Children[0]);
            }

            Presenter.Children.Add(control);

            control.Width = ActualWidth;
            control.Height = ActualHeight;

            if (presenter.ViewModel.PresenterDataContext is null ||
                !presenter.ViewModel.PresenterDataContext.Equals(dataContext))
            {
                presenter.ViewModel.PresenterDataContext = dataContext;
            }
        }
    }

    public void OnNavigatedTo()
    {
        if (!HasNavigationTo)
        {
            HasNavigationTo = true;

            var titleBar = AppCore.GetRequiredService<SearchTitleBar>();
            titleBar.CheckedChanged += OnCheckedChanged;

            if (DataContext is SearchAggregationModel model)
            {
                ViewModel.Model = model;

                DataContext = this;
            }

            var navView = AppCore.GetRequiredService<INavigationService>().GetNavigationControl() as NavigationView;

            if (navView?.Template.FindName("PART_NavigationViewContentPresenter", navView) is NavigationViewContentPresenter navPresenter)
            {
                ScrollViewer.SetCanContentScroll(navPresenter, false);
                ScrollViewer.SetHorizontalScrollBarVisibility(navPresenter, ScrollBarVisibility.Disabled);
                ScrollViewer.SetVerticalScrollBarVisibility(navPresenter, ScrollBarVisibility.Disabled);
                ScrollViewer.SetIsDeferredScrollingEnabled(navPresenter, false);
            }

            var navigationHeaderUpdater = AppCore.GetRequiredService<INavigationCustomHeaderController>();
            navigationHeaderUpdater.Show(titleBar);

            AppCore.MainWindow.SizeChanged += OnWindowSizeChanged;
        }
    }

    public void OnSearch(SearchAggregationModel model)
    {
        ViewModel.Model = model;

        var presenterViewModelsTypes = TypeResolveUtils
            .GetDerivedTypes(typeof(IPresenterViewModel<>));

        foreach (var vmType in presenterViewModelsTypes)
        {
            var method = typeof(IPresenterViewModel<>)
                .MakeGenericType(vmType.GetInterface("IPresenterViewModel`1")!.GenericTypeArguments[0])
                .GetMethod("ResetData");

            var vm = AppCore.GetService(vmType);

            method?.Invoke(vm, null);
        }

        var titleBar = AppCore.GetRequiredService<SearchTitleBar>();

        titleBar.DefaultPage.IsChecked = false;
        titleBar.DefaultPage.IsChecked = true;
    }

    public void OnNavigatedFrom()
    {
        if (HasNavigationTo)
        {
            HasNavigationTo = false;

            _loaderProvider.Loaded();

            var navigationHeaderUpdater = AppCore.GetRequiredService<INavigationCustomHeaderController>();

            navigationHeaderUpdater.Hide();

            var titleBar = AppCore.GetRequiredService<SearchTitleBar>();
            titleBar.CheckedChanged -= OnCheckedChanged;

            AppCore.MainWindow.SizeChanged -= OnWindowSizeChanged;
        }
    }

    private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (Presenter.Children.Count > 0 && Presenter.Children[0] is FrameworkElement element)
        {
            if (e.Source is Window window)
            {
                element.Width = window.WindowState is WindowState.Maximized
                    ? e.NewSize.Width - 240
                    : e.NewSize.Width <= 1000 ? e.NewSize.Width - 92 : e.NewSize.Width - 230;
            }

            element.Height = e.NewSize.Height;

            var navView = AppCore.GetRequiredService<INavigationService>().GetNavigationControl() as NavigationView;

            if (navView?.Template.FindName("PART_NavigationViewContentPresenter", navView) is NavigationViewContentPresenter navPresenter)
            {
                ScrollViewer.SetCanContentScroll(navPresenter, false);
                ScrollViewer.SetHorizontalScrollBarVisibility(navPresenter, ScrollBarVisibility.Disabled);
                ScrollViewer.SetVerticalScrollBarVisibility(navPresenter, ScrollBarVisibility.Disabled);
                ScrollViewer.SetIsDeferredScrollingEnabled(navPresenter, false);
            }
        }
    }
}