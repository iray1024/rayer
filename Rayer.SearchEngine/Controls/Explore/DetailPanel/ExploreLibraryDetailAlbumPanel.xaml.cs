using Rayer.Core.Controls;
using Rayer.Core.Framework;
using Rayer.FrameworkCore;
using Rayer.SearchEngine.Core.Domain.Album;
using Rayer.SearchEngine.ViewModels.Explore.DetailPanel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Rayer.SearchEngine.Controls.Explore.DetailPanel;

public partial class ExploreLibraryDetailAlbumPanel : UserControl
{
    private readonly Wpf.Ui.Controls.INavigationView _navigationView;

    public ExploreLibraryDetailAlbumPanel()
    {
        var vm = AppCore.GetRequiredService<ExploreLibraryDetailAlbumViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();

        _navigationView = AppCore.GetRequiredService<Wpf.Ui.INavigationService>().GetNavigationControl();

        _navigationView.PaneOpened += OnPaneOpened;
        _navigationView.PaneClosed += OnPaneClosed;
    }

    public ExploreLibraryDetailAlbumViewModel ViewModel { get; set; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged += OnSizeChanged;

        var currentWindowWidth = AppCore.MainWindow.ActualWidth;

        var factor = (currentWindowWidth + 500) / SystemParameters.PrimaryScreenWidth;

        var panelWidth = ((currentWindowWidth - (_navigationView.IsPaneOpen ? 160 : 90)) / 5) - (100 * Math.Min(factor, 1));

        ViewModel.CoverMaxWidth = _navigationView.IsPaneOpen ? panelWidth + (65 * Math.Min(1, factor)) : panelWidth + (75 * Math.Min(1, factor));
        ViewModel.CoverRectClip = new RectangleGeometry(new(0, 0, ViewModel.CoverMaxWidth, ViewModel.CoverMaxWidth), 6, 6);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged -= OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Resize(e.NewSize.Width);
    }

    private void OnPaneOpened(Wpf.Ui.Controls.NavigationView sender, RoutedEventArgs args)
    {
        Resize(AppCore.MainWindow.ActualWidth);
    }

    private void OnPaneClosed(Wpf.Ui.Controls.NavigationView sender, RoutedEventArgs args)
    {
        Resize(AppCore.MainWindow.ActualWidth);
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

    private void Resize(double newWidth)
    {
        var factor = (newWidth + 500) / SystemParameters.PrimaryScreenWidth;

        var panelWidth = ((newWidth - (_navigationView.IsPaneOpen ? 160 : 90)) / 5) - (100 * Math.Min(factor, 1));

        ViewModel.CoverMaxWidth = _navigationView.IsPaneOpen ? panelWidth + (65 * Math.Min(1, factor)) : panelWidth + (75 * Math.Min(1, factor));
        ViewModel.CoverRectClip = new RectangleGeometry(new Rect(0, 0, ViewModel.CoverMaxWidth, ViewModel.CoverMaxWidth), 6, 6);
    }

    private void OnPlaylistMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var nav = AppCore.GetRequiredService<Wpf.Ui.INavigationService>();

        var loader = AppCore.GetRequiredService<ILoaderProvider>();
        loader.Loading();

        if (sender is Grid grid)
        {
            if (grid.DataContext is Album album)
            {
                nav.Navigate(typeof(ExplorePlaylistPanel), album);
            }
        }
    }
}