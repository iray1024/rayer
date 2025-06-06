using Rayer.Core.Common;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Bilibili.Http;
using Rayer.SearchEngine.Bilibili.Models.Search;
using Rayer.SearchEngine.Core.Abstractions;
using Rayer.SearchEngine.Core.Domain.Aduio;

namespace Rayer.SearchEngine.Bilibili.Engine;

[Inject<ISearchAudioEngine>(ServiceKey = SearcherType.Bilibili)]
internal class SearchAudioEngine : SearchEngineBase, ISearchAudioEngine
{
    public SearchAudioEngine()
    {

    }

    public async Task<SearchAudio> SearchAsync(string keywords, int offset)
    {
        var result = await Searcher.GetAsync(string.Format(ApiEndpoints.Search.SearchBvId, keywords));

        var response = result.ToEntity<SearchAudioModel>();

        if (response is not null)
        {
            var domain = Mapper.Map<SearchAudio>(response);

            return domain;
        }

        return default!;
    }

    public Task<SearchAudioDetail[]> SearchDetailAsync(SearchAudioDetail[] details)
    {
        return Task.FromResult(details);
    }

    public async Task<WebAudio> GetAudioAsync(SearchAudioDetail detail)
    {
        var bvid = detail.Tags["BvId"];

        var cidResult = await Searcher.GetAsync(string.Format(ApiEndpoints.Search.SearchCId, bvid));

        var cidResponse = cidResult.ToEntity<SearchCidModel>();

        if (cidResponse is not null)
        {
            var cid = cidResponse.Data.Cid;

            detail.Tags.TryAdd("CId", cid.ToString());

            var result = await Searcher.GetAsync(string.Format(ApiEndpoints.Search.SearchUrl, bvid, cid));

            var response = result.ToEntity<WebAudioModel>();

            if (response is not null)
            {
                var domain = Mapper.Map<WebAudio>(response);

                return domain;
            }
        }

        return default!;
    }

    public async Task<WebAudio> GetAudioFromIdAsync(SearchAudioDetail detail)
    {
        var bvid = detail.Tags["BvId"];
        var id = long.Parse(detail.Tags["Id"]);

        var cidResult = await Searcher.GetAsync(string.Format(ApiEndpoints.Search.SearchCIdFromId, id));

        var cidResponse = cidResult.ToEntity<SearchCidModel>();

        if (cidResponse is not null)
        {
            var cid = cidResponse.Data.Cid;

            detail.Tags.TryAdd("CId", cid.ToString());

            var result = await Searcher.GetAsync(string.Format(ApiEndpoints.Search.SearchUrl, bvid, cid));

            var response = result.ToEntity<WebAudioModel>();

            if (response is not null)
            {
                var domain = Mapper.Map<WebAudio>(response);

                return domain;
            }
        }

        return default!;
    }
}