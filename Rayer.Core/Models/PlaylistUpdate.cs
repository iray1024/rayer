namespace Rayer.Core.Models;

public record PlaylistUpdate
{
    public int Id { get; set; }

    public Audio Target { get; set; } = null!;

    public int? To { get; set; }
}