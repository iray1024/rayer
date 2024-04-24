using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Search.Album;

public class SearchAlbumDetailInformationModel : ResponseBase
{
    public long Id { get; set; }

    public ArtistModel Artist { get; set; } = null!;

    public ArtistModel[] Artists { get; set; } = [];

    public string Name { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("picUrl")]
    public string Cover { get; set; } = string.Empty;

    public long PublishTime { get; set; }

    public int Size { get; set; }

    public string Type { get; set; } = string.Empty;

    public string SubType { get; set; } = string.Empty;
}