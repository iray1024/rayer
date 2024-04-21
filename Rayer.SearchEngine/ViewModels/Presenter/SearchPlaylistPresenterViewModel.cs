using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Core.Domain.Playlist;

namespace Rayer.SearchEngine.ViewModels.Presenter;

[Inject]
public partial class SearchPlaylistPresenterViewModel : ObservableObject, IPresenterViewModel<SearchPlaylist>
{
    [ObservableProperty]
    public SearchPlaylist _presenterDataContext = null!;
}