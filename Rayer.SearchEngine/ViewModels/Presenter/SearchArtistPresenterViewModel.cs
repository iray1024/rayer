using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Core.Domain.Artist;

namespace Rayer.SearchEngine.ViewModels.Presenter;

[Inject]
public partial class SearchArtistPresenterViewModel : ObservableObject, IPresenterViewModel<SearchArtist>
{
    [ObservableProperty]
    public SearchArtist _presenterDataContext = null!;
}