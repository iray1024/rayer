﻿using Rayer.SearchEngine.Lyric.Abstractions;
using Rayer.SearchEngine.Lyric.Models;
using Rayer.SearchEngine.Lyric.Providers.Web.QQMusic;
using Rayer.SearchEngine.Lyric.Searchers.Abstractions;

namespace Rayer.SearchEngine.Lyric.Searchers.Impl;

internal class QQMusicSearcher : Searcher, ISearcher
{
    public override string Name => "QQ Music";

    public override string DisplayName => "QQ Music";

    public override async Task<List<ISearchResult>?> SearchForResults(string searchString)
    {
        var search = new List<ISearchResult>();

        try
        {
            var result = await Providers.Web.Providers.QQMusicApi.Search(searchString, Api.SearchTypeEnum.SONG_ID);
            var results = result?.Req_1?.Data?.Body?.Song?.List;
            if (results == null)
            {
                return null;
            }

            foreach (var track in results)
            {
                search.Add(new QQMusicSearchResult(track));
                if (track.Group is { Count: > 0 } group)
                {
                    foreach (var subTrack in group)
                    {
                        search.Add(new QQMusicSearchResult(subTrack));
                    }
                }
            }
        }
        catch
        {
            return null;
        }

        return search;
    }
}