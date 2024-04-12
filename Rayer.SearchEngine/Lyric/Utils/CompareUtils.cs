using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Impl;
using Rayer.SearchEngine.Lyric.Abstractions;

namespace Rayer.SearchEngine.Lyric.Utils;

internal static partial class CompareUtils
{
    internal static MatchType CompareTrack(ITrackMetadata track, ISearchResult searchResult)
    {
        return CompareTrack(TrackMultiArtistMetadata.GetTrackMultiArtistMetadata(track), searchResult);
    }

    internal static MatchType CompareTrack(TrackMultiArtistMetadata track, ISearchResult searchResult)
    {
        var trackMatch = CompareName(track.Title, searchResult.Title);
        var artistMatch = CompareArtist(track.Artists, searchResult.Artists);
        var albumMatch = CompareName(track.Album, searchResult.Album);
        var albumArtistMatch = CompareArtist(track.AlbumArtists, searchResult.AlbumArtists);
        var durationMatch = CompareDuration(track.DurationMs, searchResult.DurationMs);

        var totalScore = 0d;
        totalScore += trackMatch.GetMatchScore();
        totalScore += artistMatch.GetMatchScore();
        totalScore += albumMatch.GetMatchScore() * 0.4;
        totalScore += albumArtistMatch.GetMatchScore() * 0.2;
        totalScore += durationMatch.GetMatchScore();

        return totalScore switch
        {
            > 21 => MatchType.Perfect,
            > 19 => MatchType.VeryHigh,
            > 17 => MatchType.High,
            > 15 => MatchType.PrettyHigh,
            > 11 => MatchType.Medium,
            > 8 => MatchType.Low,
            > 3 => MatchType.VeryLow,
            _ => MatchType.NoMatch,
        };
    }
}