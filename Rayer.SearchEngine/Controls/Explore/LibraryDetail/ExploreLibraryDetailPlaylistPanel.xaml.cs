using Rayer.Core;
using Rayer.Core.Utils;
using Rayer.SearchEngine.ViewModels.Explore.LibraryDetail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Rayer.SearchEngine.Controls.Explore.LibraryDetail;

public partial class ExploreLibraryDetailPlaylistPanel : UserControl
{
    private readonly Storyboard _transformStoryboard = new();

    public ExploreLibraryDetailPlaylistPanel()
    {
        var vm = AppCore.GetRequiredService<ExploreLibraryDetailPlaylistViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public ExploreLibraryDetailPlaylistViewModel ViewModel { get; set; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged += OnSizeChanged;

        var currentWindowWidth = AppCore.MainWindow.ActualWidth;

        var currentScreen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(Application.Current.MainWindow).Handle);

        var factor = (currentWindowWidth + 500) / currentScreen.Bounds.Width;

        var panelWidth = ((currentWindowWidth - 180) / 5) - (100 * Math.Min(factor, 1));

        ViewModel.CoverMaxWidth = panelWidth + 60;
        ViewModel.CoverRectClip = new RectangleGeometry(new(0, 0, ViewModel.CoverMaxWidth, ViewModel.CoverMaxWidth), 6, 6);

        foreach (var item in ItemGroup.Items)
        {
            var vItem = ItemGroup.ItemContainerGenerator.ContainerFromItem(item);

            var presenter = ElementHelper.FindVisualChild<Border>(vItem);

            var vBorder = presenter.FindName("PART_Border") as Border;

            if (vBorder is not null)
            {
                var coverGrid = vBorder.FindName("CoverGrid") as Grid;

                coverGrid?.SetBinding(ClipProperty, new Binding("CoverRectClip")
                {
                    Source = ViewModel
                });
            }
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged -= OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var factor = (e.NewSize.Width + 500) / SystemParameters.PrimaryScreenWidth;

        var panelWidth = ((e.NewSize.Width - 180) / 5) - (100 * Math.Min(factor, 1));

        ViewModel.CoverMaxWidth = panelWidth + 60;
        ViewModel.CoverRectClip = new RectangleGeometry(new Rect(0, 0, ViewModel.CoverMaxWidth, ViewModel.CoverMaxWidth), 6, 6);
    }

    private void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is Border border)
        {
            var cover = border.FindName("Cover") as Wpf.Ui.Controls.Image;

            if (cover is not null)
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
    }

    private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is Border border)
        {
            var cover = border.FindName("Cover") as Wpf.Ui.Controls.Image;

            if (cover is not null)
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
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(500),
                    EasingFunction = new QuadraticEase()
                };

                var animationY = new DoubleAnimation
                {
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(500),
                    EasingFunction = new QuadraticEase()
                };

                transform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
                transform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
            }
        }
    }
}