using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Controls;
using Rayer.SearchEngine.Core.Domain.Album;
using System.Windows.Media;

namespace Rayer.SearchEngine.ViewModels.Presenter;

[Inject]
public partial class SearchAlbumPresenterViewModel : ObservableObject, IPresenterViewModel<SearchAlbum>
{
    [ObservableProperty]
    private SearchAlbum _presenterDataContext = null!;

    [ObservableProperty]
    private double _coverMaxWidth = 128;

    [ObservableProperty]
    private RectangleGeometry _coverRectClip = new(new(0, 0, 128, 128), 6, 6);
}