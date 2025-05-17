using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Caching.Memory;
using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Aggregation;
using Rayer.SearchEngine.Core.Domain.Album;
using Rayer.SearchEngine.Core.Domain.Artist;
using Rayer.SearchEngine.Core.Domain.Playlist;
using Rayer.SearchEngine.Core.Domain.Video;
using Rayer.SearchEngine.Core.Enums;
using System.Net.Http;

namespace Rayer.SearchEngine.ViewModels;

[Inject]
public partial class SearchViewModel : ObservableObject
{
    private readonly IAggregationServiceProvider _engineProvider;
    private readonly IMemoryCache _cache;

    public SearchViewModel(IAggregationServiceProvider engineProvider, IMemoryCache cache)
    {
        _engineProvider = engineProvider;
        _cache = cache;
    }

    public SearchAggregationModel Model { get; set; } = null!;

    public async Task<SearchAudio> LoadAudioAsync()
    {
        var cacheKey = Model.Audio.GetHashCode();

        if (_cache.TryGetValue<SearchAudio>(cacheKey, out var response) && response is not null)
        {
            return response;
        }
        else
        {
            if (Model.Audio.Details.Length > 0)
            {
                var newResponse = await _engineProvider.AudioEngine.SearchDetailAsync(Model.Audio.Details);

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

    public Task<SearchArtist> LoadArtistAsync()
    {
        return Task.FromResult<SearchArtist>(default!);
    }

    public async Task<SearchAlbum> LoadAlbumAsync()
    {
        if (Model.Album is not null)
        {
            var cacheKey = Model.Album.GetHashCode();

            if (_cache.TryGetValue<SearchAlbum>(cacheKey, out var response) && response is not null)
            {
                return response;
            }
        }

        var model = await _engineProvider.SearchEngine.SearchAsync(Model.QueryText, SearchType.Album, AppCore.StoppingToken);

        Model.Album = model.Album;

        var newCacheKey = Model.Album.GetHashCode();

        _cache.Set(newCacheKey, Model.Album, TimeSpan.FromMinutes(10));

        return Model.Album;
    }

    public Task<SearchVideo> LoadVideoAsync()
    {
        return Task.FromResult<SearchVideo>(default!);
    }

    public Task<SearchPlaylist> LoadPlaylistAsync()
    {
        return Task.FromResult<SearchPlaylist>(default!);
    }
}