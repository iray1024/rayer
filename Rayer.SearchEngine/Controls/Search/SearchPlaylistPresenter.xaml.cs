using Rayer.FrameworkCore;
using Rayer.SearchEngine.Core.Domain.Playlist;
using Rayer.SearchEngine.ViewModels.Presenter;
using System.Windows.Controls;

namespace Rayer.SearchEngine.Controls.Search;

public partial class SearchPlaylistPresenter : UserControl, IPresenterControl<SearchPlaylistPresenterViewModel, SearchPlaylist>
{
    public SearchPlaylistPresenter()
    {
        var vm = AppCore.GetRequiredService<SearchPlaylistPresenterViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public SearchPlaylistPresenterViewModel ViewModel { get; set; }
}