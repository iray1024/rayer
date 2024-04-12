using Rayer.Core.Lyric.Abstractions;
using Rayer.SearchEngine.Lyric.Abstractions;
using Rayer.SearchEngine.Lyric.Utils;

namespace Rayer.SearchEngine.Lyric.Searchers.Abstractions;

internal abstract class Searcher : ISearcher
{
    public abstract string Name { get; }

    public abstract string DisplayName { get; }

    public abstract Task<List<ISearchResult>?> SearchForResults(string searchString);

    public async Task<ISearchResult?> SearchForResult(ITrackMetadata track)
    {
        var search = await SearchForResults(track);

        // 没有搜到时，尝试完整搜索
        if (search is not { Count: > 0 })
        {
            search = await SearchForResults(track, true);
        }

        // 仍然没有搜到，直接返回 null
        return search is not { Count: > 0 } ? null : search[0];
    }

    public async Task<ISearchResult?> SearchForResult(ITrackMetadata track, MatchType minimumMatch)
    {
        var search = await SearchForResults(track);

        // 没有搜到时，尝试完整搜索
        if (search is not { Count: > 0 } || (int)search[0].MatchType! < (int)minimumMatch)
        {
            search = await SearchForResults(track, true);
        }

        // 仍然没有搜到，直接返回 null
        return search is not { Count: > 0 } ? null : (int)search[0].MatchType! >= (int)minimumMatch ? search[0] : null;
    }

    public async Task<List<ISearchResult>> SearchForResults(ITrackMetadata track)
    {
        return await SearchForResults(track, false);
    }

    public async Task<List<ISearchResult>> SearchForResults(ITrackMetadata track, bool fullSearch)
    {
        var searchString = $"{track.Title} {track.Artist?.Replace(", ", " ")} {track.Album}".Replace(" - ", " ").Trim();
        var searchResults = new List<ISearchResult>();

        var level = 1;
        do
        {
            var results = await SearchForResults(searchString);
            if (results is { Count: > 0 })
            {
                searchResults.AddRange(results);
            }

            var newTitle = track.Title;
            if (newTitle?.Contains("(feat.") == true)
            {
                newTitle = newTitle[..newTitle.IndexOf("(feat.")].Trim();
            }

            if (newTitle?.Contains(" - feat.") == true)
            {
                newTitle = newTitle[..newTitle.IndexOf(" - feat.")].Trim();
            }

            if (fullSearch || results is not { Count: > 0 })
            {
                var newSearchString = level switch
                {
                    1 => $"{newTitle} {track.Artist?.Replace(", ", " ")}".Replace(" - ", " ").Trim(),
                    2 => $"{newTitle}".Replace(" - ", " ").Trim(),
                    _ => string.Empty,
                };
                if (newSearchString != searchString)
                {
                    searchString = newSearchString;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        } while (++level < 3);

        foreach (var result in searchResults)
        {
            result.SetMatchType(CompareUtils.CompareTrack(track, result));
        }

        searchResults.Sort((x, y) => -((int)x.MatchType!).CompareTo((int)y.MatchType!));

        return searchResults;
    }
}