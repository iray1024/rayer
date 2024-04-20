using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Business.Data.Abstractions;
using Rayer.SearchEngine.Models.Response.Netease.User;
using System.Windows.Media;

namespace Rayer.SearchEngine.ViewModels.Explore.LibraryDetail;

[Inject]
public partial class ExploreLibraryDetailPlaylistViewModel : ObservableObject
{
    private readonly IExploreLibraryDataProvider _dataProvider;

    [ObservableProperty]
    private PlaylistDetailInformation[] _playlist = [];

    [ObservableProperty]
    private double _coverMaxWidth = 128;

    [ObservableProperty]
    private RectangleGeometry _coverRectClip = new(new(0, 0, 128, 128), 6, 6);

    public ExploreLibraryDetailPlaylistViewModel()
    {
        _dataProvider = AppCore.GetRequiredService<IExploreLibraryDataProvider>();

        _dataProvider.Loaded += OnDataLoaded;
    }

    private void OnDataLoaded(object? sender, EventArgs e)
    {
        Playlist = _dataProvider.Model.Detail.Playlist;

        OnPropertyChanged(nameof(Playlist));
    }
}