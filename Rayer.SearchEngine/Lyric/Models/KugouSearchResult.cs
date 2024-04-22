using Rayer.SearchEngine.Lyric.Abstractions;
using Rayer.SearchEngine.Lyric.Providers.Web.Kugou;
using Rayer.SearchEngine.Lyric.Searchers.Abstractions;
using Rayer.SearchEngine.Lyric.Searchers.Impl;

namespace Rayer.SearchEngine.Lyric.Models;

public class KugouSearchResult(string title, string[] artists, string album, string[]? albumArtists, int durationMs, string hash) : ISearchResult
{
    public ISearcher Searcher => new KugouSearcher();

    public KugouSearchResult(SearchSongResponse.DataItem.InfoItem song) : this(
        song.SongName,
        song.SingerName.Split('、'),
        song.AlbumName,
        null,
        song.Duration * 1000,
        song.Hash
        )
    { }

    public string Title { get; } = title;

    public string[] Artists { get; } = artists;

    public string Album { get; } = album;

    public string Hash { get; } = hash;

    public string[]? AlbumArtists { get; } = albumArtists;

    public int? DurationMs { get; } = durationMs;

    public MatchType? MatchType { get; set; }
}