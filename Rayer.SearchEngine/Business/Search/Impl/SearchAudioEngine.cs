using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Business.Search.Abstractions;
using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Models.Response.Search;

namespace Rayer.SearchEngine.Business.Search.Impl;

[Inject<ISearchAudioEngine>]
internal class SearchAudioEngine : SearchEngineBase, ISearchAudioEngine
{
    public SearchAudioEngine(IServiceProvider serviceProvider) : base(serviceProvider)
    {

    }

    public async Task<SearchAudioResposne> SearchAsync(string keywords, int offset)
    {
        var result = await Searcher.GetAsync(
            Search.SearchAudio()
                .WithParam("keywords", keywords)
                .WithParam("type", "1")
                .WithParam("offset", offset.ToString())
                .Build());

        var response = result.ToEntity<SearchAudioResposne>();

        return response is not null ? response : default!;
    }

    public async Task<SearchAudioDetailResponse> SearchDetailAsync(string ids)
    {
        var result = await Searcher.GetAsync(
            Track.TrackDetail()
                .WithParam("ids", ids)
                .Build());

        var response = result.ToEntity<SearchAudioDetailResponse>();

        return response is not null ? response : default!;
    }

    public async Task<GetAudioResponse> GetAudioAsync(long id)
    {
        var result = await Searcher.GetAsync(
            Track.GetTrack()
                .WithParam("id", id.ToString())
                .Build());

        var response = result.ToEntity<GetAudioResponse>();

        return response is not null ? response : default!;
    }
}