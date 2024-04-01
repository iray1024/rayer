using Rayer.Core.Models;

namespace Rayer.Core.Playing;

public class Playlist
{
    public string Name { get; set; } = string.Empty;

    public string Cover { get; set; } = string.Empty;

    public ICollection<Audio> Audios { get; set; } = [];
}