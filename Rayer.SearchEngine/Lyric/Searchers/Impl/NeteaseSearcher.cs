using Rayer.SearchEngine.Lyric.Abstractions;
using Rayer.SearchEngine.Lyric.Models;
using Rayer.SearchEngine.Lyric.Providers.Web.Netease;
using Rayer.SearchEngine.Lyric.Searchers.Abstractions;

namespace Rayer.SearchEngine.Lyric.Searchers.Impl;

internal class NeteaseSearcher : Searcher, ISearcher
{
    public override string Name => "Netease";

    public override string DisplayName => "Netease Cloud Music";

    public override async Task<List<ISearchResult>?> SearchForResults(string searchString)
    {
        var search = new List<ISearchResult>();

        try
        {
            var result = await Providers.Web.Providers.NeteaseApi.Search(searchString, Api.SearchTypeEnum.SONG_ID);
            var results = result?.Result.Songs;
            if (results == null)
            {
                return null;
            }

            foreach (var track in results)
            {
                search.Add(new NeteaseSearchResult(track));
            }
        }
        catch
        {
            return null;
        }

        return search;
    }
}