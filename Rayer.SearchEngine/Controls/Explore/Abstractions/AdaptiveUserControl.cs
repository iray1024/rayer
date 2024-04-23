using Rayer.Core;
using Rayer.SearchEngine.ViewModels.Explore.Abstractions;
using System.Windows;
using System.Windows.Controls;

namespace Rayer.SearchEngine.Controls.Explore.Abstractions;

public abstract class AdaptiveUserControl(AdaptiveAudioListViewModelBase viewModel) : UserControl
{
    protected AdaptiveAudioListViewModelBase ViewModel { get; set; } = viewModel;

    protected virtual void OnLoaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged += OnSizeChanged;

        var panelWidth = (AppCore.MainWindow.ActualWidth - 180 - ((int)ActualWidth >> 1)) / 3;

        ViewModel.NameMaxWidth = panelWidth + 50;
        ViewModel.ArtistsNameMaxWidth = panelWidth + 50;
        ViewModel.AlbumNameMaxHeight = panelWidth + 80;

        ViewModel.DurationMaxHeight = e.Source is Window { WindowState: WindowState.Maximized } ? 43 : 39;
        ViewModel.ItemMargin = e.Source is Window { WindowState: WindowState.Maximized }
            ? new Thickness(0, 0, 30, 0)
            : new Thickness(0, 0, 24, 0);

        AppCore.MainWindow.Width += 1;
        AppCore.MainWindow.Width -= 1;
    }

    protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged -= OnSizeChanged;

        ViewModel = default!;
    }

    protected virtual void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var panelWidth = (e.NewSize.Width - 180 - ((int)ActualWidth >> 1)) / 3;

        ViewModel.NameMaxWidth = panelWidth + 50;
        ViewModel.ArtistsNameMaxWidth = panelWidth + 50;
        ViewModel.AlbumNameMaxHeight = panelWidth + 80;

        ViewModel.DurationMaxHeight = e.Source is Window { WindowState: WindowState.Maximized } ? 43 : 39;
        ViewModel.ItemMargin = e.Source is Window { WindowState: WindowState.Maximized }
            ? new Thickness(0, 0, 30, 0)
            : new Thickness(0, 0, 24, 0);
    }
}