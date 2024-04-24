using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Search.Album;

public class SearchAlbumModel : ResponseBase
{
    public SearchAlbumDetailModel Result { get; set; } = null!;

    public record SearchAlbumDetailModel
    {
        public SearchAlbumDetailInformationModel[] Albums { get; set; } = [];

        [JsonPropertyName("albumCount")]
        public int Count { get; set; }
    }
}