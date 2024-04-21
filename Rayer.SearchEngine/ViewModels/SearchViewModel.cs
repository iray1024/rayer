using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Caching.Memory;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Abstractions.Provider;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Aggregation;
using Rayer.SearchEngine.Core.Domain.Album;
using Rayer.SearchEngine.Core.Domain.Artist;
using Rayer.SearchEngine.Core.Domain.Playlist;
using Rayer.SearchEngine.Core.Domain.Video;
using System.Net.Http;

namespace Rayer.SearchEngine.ViewModels;

[Inject]
public partial class SearchViewModel : ObservableObject
{
    private readonly ISearchAudioEngineProvider _audioEngineProvider;
    private readonly IMemoryCache _cache;

    public SearchViewModel(ISearchAudioEngineProvider audioEngineProvider, IMemoryCache cache)
    {
        _audioEngineProvider = audioEngineProvider;
        _cache = cache;
    }

    public SearchAggregationModel Model { get; set; } = null!;

    public async Task<SearchAudio> LoadAudioAsync()
    {
        var cacheKey = Model.GetHashCode();

        if (_cache.TryGetValue<SearchAudio>(cacheKey, out var response) && response is not null)
        {
            return response;
        }
        else
        {
            if (Model.Audio.Details.Length > 0)
            {
                var ids = string.Join(',', Model.Audio.Details.Select(x => x.Id));

                var newResponse = await _audioEngineProvider.AudioEngine.SearchDetailAsync(ids);

                Model.Audio.Details = newResponse;

                _cache.Set(cacheKey, Model.Audio, TimeSpan.FromMinutes(10));

                return Model.Audio;
            }
            else
            {
                throw new HttpRequestException("请求服务器失败");
            }
        }
    }

    public async Task<SearchArtist> LoadArtistAsync()
    {

        return default!;
    }

    public async Task<SearchAlbum> LoadAlbumAsync()
    {
        return default!;
    }

    public async Task<SearchVideo> LoadVideoAsync()
    {
        return default!;
    }

    public async Task<SearchPlaylist> LoadPlaylistAsync()
    {
        return default!;
    }
}