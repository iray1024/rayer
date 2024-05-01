using Rayer.Core;
using Rayer.Core.Controls;
using Rayer.Core.Framework;
using Rayer.Core.Utils;
using Rayer.SearchEngine.Controls.Explore;
using Rayer.SearchEngine.Core.Domain.Album;
using Rayer.SearchEngine.ViewModels.Presenter;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Rayer.SearchEngine.Controls.Search;

public partial class SearchAlbumPresenter : UserControl, IPresenterControl<SearchAlbumPresenterViewModel, SearchAlbum>
{
    private readonly Wpf.Ui.Controls.INavigationView _navigationView;

    private bool _isLoaded = false;

    public SearchAlbumPresenter()
    {
        var vm = AppCore.GetRequiredService<SearchAlbumPresenterViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();

        _navigationView = AppCore.GetRequiredService<Wpf.Ui.INavigationService>().GetNavigationControl();
    }

    public SearchAlbumPresenterViewModel ViewModel { get; set; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged += OnSizeChanged;

        if (!_isLoaded)
        {
            ((Grid)Parent).SizeChanged += OnParentSizeChanged;
            _isLoaded = true;
        }

        BindingOperations.SetBinding(ItemGroup, ItemsControl.ItemsSourceProperty, new Binding()
        {
            Source = ViewModel.PresenterDataContext.Details
        });

        var currentWindowWidth = AppCore.MainWindow.ActualWidth;

        Resize(currentWindowWidth);

        var navView = AppCore.GetRequiredService<INavigationService>().GetNavigationControl() as NavigationView;

        if (navView?.Template.FindName("PART_NavigationViewContentPresenter", navView) is NavigationViewContentPresenter navPresenter)
        {
            var scrollViewer = ElementHelper.GetScrollViewer(navPresenter);

            scrollViewer?.ScrollToTop();
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged -= OnSizeChanged;

        ItemGroup.ItemsSource = null;

        BindingOperations.ClearAllBindings(this);

        GC.Collect();
    }

    #region Effect    
    private void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is AsyncImage cover)
        {
            var center = (int)ViewModel.CoverMaxWidth >> 1;

            var transform = new ScaleTransform()
            {
                CenterX = center,
                CenterY = center
            };

            cover.RenderTransform = transform;

            var animationX = new DoubleAnimation
            {
                To = 1.2,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase()
            };

            var animationY = new DoubleAnimation
            {
                To = 1.2,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase()
            };

            transform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
        }
    }

    private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        if (sender is AsyncImage cover)
        {
            var center = (int)ViewModel.CoverMaxWidth >> 1;

            var transform = new ScaleTransform()
            {
                CenterX = center,
                CenterY = center
            };
            cover.RenderTransform = transform;

            var animationX = new DoubleAnimation
            {
                From = 1.2,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase()
            };

            var animationY = new DoubleAnimation
            {
                From = 1.2,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase()
            };

            transform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
        }
    }
    #endregion

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Resize(e.NewSize.Width);
    }

    private void Resize(double newWidth)
    {
        var factor = (newWidth + 500) / SystemParameters.PrimaryScreenWidth;

        var panelWidth = ((newWidth - (_navigationView.IsPaneOpen ? 160 : 90)) / 5) - (100 * Math.Min(factor, 1));

        ViewModel.CoverMaxWidth = _navigationView.IsPaneOpen ? panelWidth + (65 * Math.Min(1, factor)) : panelWidth + (75 * Math.Min(1, factor));
        ViewModel.CoverRectClip = new RectangleGeometry(new Rect(0, 0, ViewModel.CoverMaxWidth, ViewModel.CoverMaxWidth), 6, 6);

        if (ViewModel.PresenterDataContext is not null)
        {
            Height = (ViewModel.CoverMaxWidth + 50) * Math.Ceiling(1.0 * ViewModel.PresenterDataContext.Details.Length / 5);
        }
    }

    private void OnParentSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Width = e.NewSize.Width;

        Resize(AppCore.MainWindow.ActualWidth);
    }

    private void OnAlbumMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var nav = AppCore.GetRequiredService<Wpf.Ui.INavigationService>();

        var loader = AppCore.GetRequiredService<ILoaderProvider>();
        loader.Loading();

        if (sender is Grid grid)
        {
            if (grid.DataContext is SearchAlbumDetail album)
            {
                nav.Navigate(typeof(ExplorePlaylistPanel), album);
            }
        }
    }
}