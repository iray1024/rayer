using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Caching.Memory;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Business.Search.Abstractions;
using Rayer.SearchEngine.Models.Response.Search;

namespace Rayer.SearchEngine.ViewModels;

[Inject]
public partial class SearchViewModel : ObservableObject
{
    private readonly ISearchAudioEngine _audioEngine;
    private readonly IMemoryCache _cache;

    public SearchViewModel(ISearchAudioEngine audioEngine, IMemoryCache cache)
    {
        _audioEngine = audioEngine;
        _cache = cache;
    }

    public SearchAggregationModel Model { get; set; } = null!;

    public async Task<SearchAudioDetailResponse> LoadAudioAsync()
    {
        if (_cache.TryGetValue<SearchAudioDetailResponse>(Model.Audio, out var response) && response is not null)
        {
            return response;
        }
        else
        {
            var ids = string.Join(',', Model.Audio.Result.Songs.Select(x => x.Id));

            var newResponse = await _audioEngine.SearchDetailAsync(ids);

            _cache.Set(Model.Audio, newResponse, TimeSpan.FromMinutes(10));

            return newResponse;
        }
    }

    public async Task<SearchSingerDetailResponse> LoadSingerAsync()
    {

        return default!;
    }

    public async Task<SearchAlbumDetailResponse> LoadAlbumAsync()
    {
        return default!;
    }

    public async Task<SearchVideoDetailResponse> LoadVideoAsync()
    {
        return default!;
    }

    public async Task<SearchPlaylistDetailResponse> LoadPlaylistAsync()
    {
        return default!;
    }
}