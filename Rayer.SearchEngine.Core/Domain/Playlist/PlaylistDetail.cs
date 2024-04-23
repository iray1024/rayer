using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Authority;

namespace Rayer.SearchEngine.Core.Domain.Playlist;

public class PlaylistDetail
{
    public long Id { get; set; }

    public long OwnerId { get; set; }

    public string Title { get; set; } = string.Empty;

    public User Creator { get; set; } = null!;

    public DateTime CreateTime { get; set; }

    public DateTime UpdateTime { get; set; }

    public string Cover { get; set; } = string.Empty;

    public int AudioCount { get; set; }

    public long PlayCount { get; set; }

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