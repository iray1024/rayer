using Rayer.Core.Lyric.Abstractions;

namespace Rayer.Core.Lyric.Impl;

public class TrackMultiArtistMetadata : ITrackMetadata
{
    public string? Title { get; set; }

    public string? Artist
    {
        get => string.Join(", ", Artists);
        set => Artists = (value ?? string.Empty).Split(", ").ToList();
    }

    public List<string> Artists { get; set; } = [];

    public string? Album { get; set; }

    public string? AlbumArtist
    {
        get => string.Join(", ", AlbumArtists);
        set => AlbumArtists = (value ?? string.Empty).Split(", ").ToList();
    }

    public List<string> AlbumArtists { get; set; } = [];

    public int? DurationMs { get; set; }

    public string? Isrc { get; set; }

    public List<string>? Language { get; set; }

    public static TrackMultiArtistMetadata GetTrackMultiArtistMetadata(ITrackMetadata track)
    {
        return track is TrackMultiArtistMetadata trackMultiArtist
            ? trackMultiArtist
            : new TrackMultiArtistMetadata
            {
                Artist = track.Artist,
                Album = track.Album,
                AlbumArtist = track.AlbumArtist,
                DurationMs = track.DurationMs,
                Isrc = track.Isrc,
                Language = track.Language,
                Title = track.Title
            };
    }
}