using Rayer.Core.Lyric.Abstractions;

namespace Rayer.Core.Lyric.Impl;

internal class TrackMetadata : ITrackMetadata
{
    public string? Title { get; set; }

    public string? Artist { get; set; }

    public string? Album { get; set; }

    public string? AlbumArtist { get; set; }

    public int? DurationMs { get; set; }

    public string? Isrc { get; set; }

    public List<string>? Language { get; set; }
}