using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Business.Search.Abstractions;
using Rayer.SearchEngine.Models.Response.Search;

namespace Rayer.SearchEngine.ViewModels;

[Inject]
public partial class SearchViewModel : ObservableObject
{
    private readonly ISearchAudioEngine _audioEngine;

    public SearchViewModel(ISearchAudioEngine audioEngine)
    {
        _audioEngine = audioEngine;
    }

    public SearchAggregationModel Model { get; set; } = null!;

    public async Task<SearchAudioDetailResponse> LoadAudioAsync()
    {
        var ids = string.Join(',', Model.Audio.Result.Songs.Select(x => x.Id));

        return await _audioEngine.SearchDetailAsync(ids);
    }

    public async Task LoadSingerAsync()
    {

    }

    public async Task LoadAlbumAsync()
    {

    }

    public async Task LoadVideoAsync()
    {

    }

    public async Task LoadPlaylistAsync()
    {

    }
}