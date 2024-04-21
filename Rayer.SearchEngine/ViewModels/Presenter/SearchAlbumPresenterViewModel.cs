using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Core.Domain.Album;

namespace Rayer.SearchEngine.ViewModels.Presenter;

[Inject]
public partial class SearchAlbumPresenterViewModel : ObservableObject, IPresenterViewModel<SearchAlbum>
{
    [ObservableProperty]
    private SearchAlbum _presenterDataContext = null!;
}