using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Abstractions;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Netease.Extensions;
using Rayer.SearchEngine.Netease.Models.Search.Audio;

namespace Rayer.SearchEngine.Netease.Engine;

[Inject<ISearchAudioEngine>(ServiceKey = SearcherType.Netease)]
internal class SearchAudioEngine : SearchEngineBase, ISearchAudioEngine
{
    public SearchAudioEngine()
    {

    }

    public async Task<SearchAudio> SearchAsync(string keywords, int offset)
    {
        var result = await Searcher.GetAsync(
            SearchSelector.SampleSearch()
                .WithParam("keywords", keywords)
                .WithParam("type", "1")
                .WithParam("offset", offset.ToString())
                .Build());

        var response = result.ToEntity<SearchAudioModel>();

        if (response is not null)
        {
            var domain = Mapper.Map<SearchAudio>(response);

            return domain;
        }

        return default!;
    }

    public async Task<SearchAudioDetail[]> SearchDetailAsync(SearchAudioDetail[] details)
    {
        var ids = string.Join(',', details.Select(x => x.Id));

        var result = await Searcher.GetAsync(
            TrackSelector.TrackDetail()
                .WithParam("ids", ids)
                .Build());

        var response = result.ToEntity<SearchAudioDetailModel>();

        if (response is not null)
        {
            foreach (var detail in response.Details)
            {
                if (detail.Album is not null)
                {
                    detail.Album.Cover += "?param=512y512";
                }
            }

            var domain = Mapper.Map<SearchAudioDetail[]>(response.Details);

            for (var i = 0; i < domain.Length; i++)
            {
                if (!CopyrightExtensions.Playable(response.Details[i], response.Privileges[i], out var reason))
                {
                    if (!string.IsNullOrEmpty(reason))
                    {
                        domain[i].Copyright = new Core.Domain.Common.Copyright { Reason = reason };
                    }
                }
            }

            return domain;
        }

        return default!;
    }

    public async Task<WebAudio> GetAudioAsync(SearchAudioDetail detail)
    {
        var result = await Searcher.GetAsync(
            TrackSelector.GetTrack()
                .WithParam("id", detail.Id.ToString())
                .Build());

        var response = result.ToEntity<WebAudioModel>();

        if (response is not null)
        {
            var domain = Mapper.Map<WebAudio>(response);

            return domain;
        }

        return default!;
    }
}