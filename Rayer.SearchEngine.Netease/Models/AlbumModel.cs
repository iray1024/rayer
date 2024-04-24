namespace Rayer.SearchEngine.Netease.Models;

public record AlbumModel
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("picUrl")]
    public string Picture { get; set; } = string.Empty;
}