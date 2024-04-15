using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Models.Response.Search;

namespace Rayer.SearchEngine.ViewModels.Presenter;

[Inject]
public partial class SearchAlbumPresenterViewModel : ObservableObject, IPresenterViewModel<SearchAlbumDetailResponse>
{
    [ObservableProperty]
    private SearchAlbumDetailResponse _presenterDataContext = null!;
}