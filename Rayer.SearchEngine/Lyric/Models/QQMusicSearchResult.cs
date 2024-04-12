using Rayer.SearchEngine.Lyric.Abstractions;
using Rayer.SearchEngine.Lyric.Impl;
using Rayer.SearchEngine.Lyric.Providers.Web.QQMusic;

namespace Rayer.SearchEngine.Lyric.Models;

public class QQMusicSearchResult : ISearchResult
{
    public ISearcher Searcher => new QQMusicSearcher();

    public QQMusicSearchResult(string title, string[] artists, string album, string[]? albumArtists, int durationMs, int id, string mid)
    {
        Title = title;
        Artists = artists;
        Album = album;
        AlbumArtists = albumArtists;
        DurationMs = durationMs;
        Id = id;
        Mid = mid;
    }

    public QQMusicSearchResult(Song song) : this(
        song.Title,
        song.Singer.Select(s => s.Title).ToArray(),
        song.Album.Title,
        null,
        song.Interval * 1000,
        (int)song.Id,
        song.Mid
        )
    { }

    public string Title { get; }

    public string[] Artists { get; }

    public string Album { get; }

    public int Id { get; }

    public string Mid { get; }

    public string[]? AlbumArtists { get; }

    public int? DurationMs { get; }

    public MatchType? MatchType { get; set; }
}