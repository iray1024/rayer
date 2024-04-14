using Rayer.Core;
using Rayer.SearchEngine.Models.Response.Search;
using Rayer.SearchEngine.ViewModels.Presenter;
using System.Windows.Controls;

namespace Rayer.SearchEngine.Controls.Search;

public partial class SearchAudioPresenter : UserControl, IPresenterControl<SearchAudioPresenterViewModel, SearchAudioDetailResponse>
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
            listViewItem.DataContext is SearchAudioDetailAudioDetail item)
        {
            await ViewModel.PlayWebAudio(item);
        }
    }

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {

    }
}