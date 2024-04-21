using Rayer.Core;
using Rayer.SearchEngine.Core.Domain.Album;
using Rayer.SearchEngine.ViewModels.Presenter;
using System.Windows.Controls;

namespace Rayer.SearchEngine.Controls.Search;

public partial class SearchAlbumPresenter : UserControl, IPresenterControl<SearchAlbumPresenterViewModel, SearchAlbum>
{
    public SearchAlbumPresenter()
    {
        var vm = AppCore.GetRequiredService<SearchAlbumPresenterViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public SearchAlbumPresenterViewModel ViewModel { get; set; }
}