using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Netease.Models.Search.Audio;

namespace Rayer.SearchEngine.Netease.Models.Search.Album;

public class AlbumDetailModel : ResponseBase
{
    public SearchAlbumDetailInformationModel Album { get; set; } = null!;

    [JsonPropertyName("songs")]
    public SearchAudioDetailInformationModel[] Audios { get; set; } = [];
}