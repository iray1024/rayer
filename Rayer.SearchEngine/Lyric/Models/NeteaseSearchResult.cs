﻿using Rayer.SearchEngine.Lyric.Abstractions;
using Rayer.SearchEngine.Lyric.Providers.Web.Netease;
using Rayer.SearchEngine.Lyric.Searchers.Abstractions;
using Rayer.SearchEngine.Lyric.Searchers.Impl;

namespace Rayer.SearchEngine.Lyric.Models;

public class NeteaseSearchResult : ISearchResult
{
    public ISearcher Searcher => new NeteaseSearcher();

    public NeteaseSearchResult(string title, string[] artists, string album, string[]? albumArtists, int durationMs, int id)
    {
        Title = title;
        Artists = artists;
        Album = album;
        AlbumArtists = albumArtists;
        DurationMs = durationMs;
        Id = id;
    }

    public NeteaseSearchResult(Song song) : this(
        song.Name,
        song.Artists.Select(s => s.Name).ToArray(),
        song.Album.Name,
        null,
        (int)song.Duration,
        (int)song.Id)
    {

    }

    public string Title { get; }

    public string[] Artists { get; }

    public string Album { get; }

    public int Id { get; }

    public string[]? AlbumArtists { get; }

    public int? DurationMs { get; }

    public MatchType? MatchType { get; set; }
}