using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Core.Domain.Video;

namespace Rayer.SearchEngine.ViewModels.Presenter;

[Inject]
public partial class SearchVideoPresenterViewModel : ObservableObject, IPresenterViewModel<SearchVideo>
{
    [ObservableProperty]
    private SearchVideo _presenterDataContext = null!;
}