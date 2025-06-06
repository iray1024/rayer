using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Business.Data;
using System.Windows.Media;

namespace Rayer.SearchEngine.ViewModels.Explore.DetailPanel;

[Inject]
public partial class ExploreLibraryDetailAlbumViewModel : ObservableObject
{
    private readonly IExploreLibraryDataProvider _dataProvider;

    [ObservableProperty]
    private Core.Domain.Album.Album[] _favAlbum = [];

    [ObservableProperty]
    private double _coverMaxWidth = 128;

    [ObservableProperty]
    private RectangleGeometry _coverRectClip = new(new(0, 0, 128, 128), 6, 6);

    public ExploreLibraryDetailAlbumViewModel()
    {
        _dataProvider = AppCore.GetRequiredService<IExploreLibraryDataProvider>();

        _dataProvider.Loaded += OnDataLoaded;
    }

    private void OnDataLoaded(object? sender, EventArgs e)
    {
        FavAlbum = _dataProvider.Model.Detail.FavAlbum;

        OnPropertyChanged(nameof(FavAlbum));
    }
}