namespace Rayer.Core.Lyric.Abstractions;

public interface ITrackMetadata
{
    public string? Title { get; set; }

    public string? Artist { get; set; }

    public string? Album { get; set; }

    public string? AlbumArtist { get; set; }

    public int? DurationMs { get; set; }

    public string? Isrc { get; set; }

    public List<string>? Language { get; set; }
}