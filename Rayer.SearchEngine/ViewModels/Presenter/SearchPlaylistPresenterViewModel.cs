using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Models.Response.Netease.Search;

namespace Rayer.SearchEngine.ViewModels.Presenter;

[Inject]
public partial class SearchPlaylistPresenterViewModel : ObservableObject, IPresenterViewModel<SearchPlaylistDetail>
{
    [ObservableProperty]
    public SearchPlaylistDetail _presenterDataContext = null!;
}