using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Caching.Memory;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Business.Search.Abstractions;
using Rayer.SearchEngine.Models.Response.Search;
using System.Net.Http;

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

    public async Task<SearchAudioDetail> LoadAudioAsync()
    {
        if (_cache.TryGetValue<SearchAudioDetail>(Model.Audio, out var response) && response is not null)
        {
            return response;
        }
        else
        {
            if (Model.Audio.Code == 200)
            {
                var ids = string.Join(',', Model.Audio.Result.Songs.Select(x => x.Id));

                var newResponse = await _audioEngine.SearchDetailAsync(ids);

                _cache.Set(Model.Audio, newResponse, TimeSpan.FromMinutes(10));

                return newResponse;
            }
            else
            {
                throw new HttpRequestException("请求服务器失败");
            }
        }
    }

    public async Task<SearchSingerDetail> LoadSingerAsync()
    {

        return default!;
    }

    public async Task<SearchAlbumDetail> LoadAlbumAsync()
    {
        return default!;
    }

    public async Task<SearchVideoDetail> LoadVideoAsync()
    {
        return default!;
    }

    public async Task<SearchPlaylistDetail> LoadPlaylistAsync()
    {
        return default!;
    }
}