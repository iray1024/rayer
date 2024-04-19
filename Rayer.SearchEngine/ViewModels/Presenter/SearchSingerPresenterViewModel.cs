using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Models.Response.Search;

namespace Rayer.SearchEngine.ViewModels.Presenter;

[Inject]
public partial class SearchSingerPresenterViewModel : ObservableObject, IPresenterViewModel<SearchSingerDetail>
{
    [ObservableProperty]
    public SearchSingerDetail _presenterDataContext = null!;
}