using Rayer.Core;
using Rayer.SearchEngine.Core.Domain.Artist;
using Rayer.SearchEngine.ViewModels.Presenter;
using System.Windows.Controls;

namespace Rayer.SearchEngine.Controls.Search;

public partial class SearchArtistPresenter : UserControl, IPresenterControl<SearchArtistPresenterViewModel, SearchArtist>
{
    public SearchArtistPresenter()
    {
        var vm = AppCore.GetRequiredService<SearchArtistPresenterViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public SearchArtistPresenterViewModel ViewModel { get; set; }
}