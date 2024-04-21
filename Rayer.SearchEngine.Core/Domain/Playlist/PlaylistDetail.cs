using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Authority;

namespace Rayer.SearchEngine.Core.Domain.Playlist;

public class PlaylistDetail
{
    public long Id { get; set; }

    public long OwnerId { get; set; }

    public string Title { get; set; } = string.Empty;

    public User Creator { get; set; } = null!;

    public string Cover { get; set; } = string.Empty;

    public int AudioCount { get; set; }

    public SearchAudioDetail[] Audios { get; set; } = [];
}