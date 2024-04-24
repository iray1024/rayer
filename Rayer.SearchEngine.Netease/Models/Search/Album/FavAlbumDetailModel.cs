namespace Rayer.SearchEngine.Netease.Models.Search.Album;

internal record FavAlbumDetailModel
{
    public long Id { get; set; }

    public string[] Alias { get; set; } = [];

    public ArtistModel[] Artists { get; set; } = [];

    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("picUrl")]
    public string Cover { get; set; } = string.Empty;

    public int Size { get; set; }
}