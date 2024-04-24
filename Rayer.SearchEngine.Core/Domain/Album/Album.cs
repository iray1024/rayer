using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Artist;
using Rayer.SearchEngine.Core.Enums;

namespace Rayer.SearchEngine.Core.Domain.Album;

public record Album : AlbumDefinition
{
    public ArtistDefinition Artist { get; set; } = null!;

    public DateTime PublishTime { get; set; }

    public string? Description { get; set; }

    public int TotalMinutes { get; set; }

    public string? Company { get; set; }

    public int AudioCount { get; set; }

    public long PlayCount { get; set; }

    public SearchType Type { get; set; } = SearchType.Album;

    public SearchAudioDetail[] Audios { get; set; } = [];

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Id,
            Title,
            AudioCount);
    }
}