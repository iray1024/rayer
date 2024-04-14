using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Models.Response.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rayer.SearchEngine.ViewModels.Presenter;

[Inject]
public partial class SearchPlaylistPresenterViewModel : ObservableObject, IPresenterViewModel<SearchPlaylistDetailResponse>
{
    [ObservableProperty]
    public SearchPlaylistDetailResponse _presenterDataContext = null!;
}