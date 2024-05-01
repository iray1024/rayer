namespace Rayer.Core.Models;

public class Playlist
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public IList<Audio> Audios { get; set; } = [];

    public int Sort { get; set; }
}