using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Authority;
using Rayer.SearchEngine.Core.Enums;

namespace Rayer.SearchEngine.Core.Domain.Playlist;

public record PlaylistDetail
{
    public long Id { get; set; }

    public long OwnerId { get; set; }

    public string Title { get; set; } = string.Empty;

    public User Creator { get; set; } = null!;

    public DateTime CreateTime { get; set; }

    public DateTime UpdateTime { get; set; }

    public string Cover { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int TotalMinutes { get; set; }

    public string? Company { get; set; }

    public int AudioCount { get; set; }

    public long PlayCount { get; set; }

    public SearchType Type { get; set; } = SearchType.Playlist;

    public SearchAudioDetail[] Audios { get; set; } = [];

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Id,
            Title,
            UpdateTime,
            AudioCount);
    }
}