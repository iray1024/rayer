using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Business.Search.Abstractions;
using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Models.Response.Netease.Search;

namespace Rayer.SearchEngine.Business.Search.Impl;

[Inject<ISearchAudioEngine>]
internal class SearchAudioEngine : SearchEngineBase, ISearchAudioEngine
{
    public SearchAudioEngine(IServiceProvider serviceProvider) : base(serviceProvider)
    {

    }

    public async Task<SearchAudio> SearchAsync(string keywords, int offset)
    {
        var result = await Searcher.GetAsync(
            Search.SearchAudio()
                .WithParam("keywords", keywords)
                .WithParam("type", "1")
                .WithParam("offset", offset.ToString())
                .Build());

        var response = result.ToEntity<SearchAudio>();

        return response is not null ? response : default!;
    }

    public async Task<SearchAudioDetail> SearchDetailAsync(string ids)
    {
        var result = await Searcher.GetAsync(
            Track.TrackDetail()
                .WithParam("ids", ids)
                .Build());

        var response = result.ToEntity<SearchAudioDetail>();

        if (response is not null)
        {
            foreach (var detail in response.Details)
            {
                if (detail.Album is not null)
                {
                    detail.Album.Picture += "?param=512y512";
                }
            }

            return response;
        }

        return default!;
    }

    public async Task<WebAudio> GetAudioAsync(long id)
    {
        var result = await Searcher.GetAsync(
            Track.GetTrack()
                .WithParam("id", id.ToString())
                .Build());

        var response = result.ToEntity<WebAudio>();

        return response is not null ? response : default!;
    }
}