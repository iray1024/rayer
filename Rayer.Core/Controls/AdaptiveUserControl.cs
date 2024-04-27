using System.Windows;
using System.Windows.Controls;

namespace Rayer.Core.Controls;

public abstract class AdaptiveUserControl(AdaptiveViewModelBase viewModel) : UserControl
{
    protected AdaptiveViewModelBase ViewModel { get; set; } = viewModel;

    protected virtual void OnLoaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged += OnSizeChanged;

        Resize(AppCore.MainWindow.ActualWidth, e);

        AppCore.MainWindow.Width += 1;
        AppCore.MainWindow.Width -= 1;
    }

    protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged -= OnSizeChanged;
    }

    protected virtual void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Resize(e.NewSize.Width, e);
    }

    protected void Resize(double newWidth, RoutedEventArgs e)
    {
        var panelWidth = (newWidth - 180 - ((int)ActualWidth >> 1)) / 3;

        ViewModel.TitleMaxWidth = panelWidth + 50;
        ViewModel.ArtistsNameMaxWidth = panelWidth + 50;
        ViewModel.AlbumNameMaxWidth = panelWidth + 80;

        ViewModel.DurationMaxWidth = e.Source is Window { WindowState: WindowState.Maximized } ? 43 : 39;
        ViewModel.ItemMargin = e.Source is Window { WindowState: WindowState.Maximized }
            ? new Thickness(0, 0, 30, 0)
            : new Thickness(0, 0, 24, 0);
    }
}