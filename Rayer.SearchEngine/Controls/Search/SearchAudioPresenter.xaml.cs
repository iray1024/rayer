using Rayer.Core;
using Rayer.SearchEngine.Models.Response.Netease.Search;
using Rayer.SearchEngine.ViewModels.Presenter;
using System.Windows;
using System.Windows.Controls;

namespace Rayer.SearchEngine.Controls.Search;

public partial class SearchAudioPresenter : UserControl, IPresenterControl<SearchAudioPresenterViewModel, SearchAudioDetail>
{
    public SearchAudioPresenter()
    {
        var vm = AppCore.GetRequiredService<SearchAudioPresenterViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public SearchAudioPresenterViewModel ViewModel { get; set; } = null!;

    private void OnListViewItemRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }

    private async void OnListViewItemDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.Source is ListViewItem listViewItem &&
            listViewItem.DataContext is SearchAudioDetailInformation item)
        {
            await ViewModel.PlayWebAudio(item);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged += OnSizeChanged;

        var panelWidth = (AppCore.MainWindow.ActualWidth - 180 - ((int)Width >> 1)) / 3;

        ViewModel.NameMaxWidth = panelWidth + 50;
        ViewModel.ArtistsNameMaxWidth = panelWidth + 50;
        ViewModel.AlbumNameMaxHeight = panelWidth + 80;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged -= OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var panelWidth = (e.NewSize.Width - 180 - ((int)Width >> 1)) / 3;

        ViewModel.NameMaxWidth = panelWidth + 50;
        ViewModel.ArtistsNameMaxWidth = panelWidth + 50;
        ViewModel.AlbumNameMaxHeight = panelWidth + 80;

        ViewModel.DurationMaxHeight = e.Source is Window { WindowState: WindowState.Maximized } ? 43 : 39;
        ViewModel.ItemMargin = e.Source is Window { WindowState: WindowState.Maximized }
            ? new Thickness(0, 0, 30, 0)
            : new Thickness(0, 0, 24, 0);
    }
}