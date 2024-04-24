namespace Rayer.SearchEngine.Core.Domain.Album;

public record AlbumDefinition
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Cover { get; set; } = string.Empty;
}