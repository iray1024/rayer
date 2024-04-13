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
        var qrCodeResult = await Searcher.GetAsync(
            Search.SearchAudio()
                .WithParam("keywords", keywords)
                .WithParam("type", "1")
                .WithParam("offset", offset.ToString())
                .Build());

        var response = qrCodeResult.ToEntity<SearchAudioResposne>();

        return response is not null ? response : default!;
    }

    public async Task<SearchAudioDetailResponse> SearchDetailAsync(string ids)
    {
        var qrCodeResult = await Searcher.GetAsync(
            Track.TrackDetail()
                .WithParam("ids", ids)
                .Build());

        var response = qrCodeResult.ToEntity<SearchAudioDetailResponse>();

        return response is not null ? response : default!;
    }
}