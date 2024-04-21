using Rayer.SearchEngine.Core.Domain.Common;

namespace Rayer.SearchEngine.Core.Domain.Playlist;

public class PlaylistDefinition
{
    public PlaylistDetail Details { get; set; } = null!;

    public Copyright Copyright { get; set; } = Copyright.Default;
}