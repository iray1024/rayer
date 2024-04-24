namespace Rayer.SearchEngine.Core.Domain.Artist;

public record ArtistDefinition
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Cover { get; set; } = string.Empty;
}