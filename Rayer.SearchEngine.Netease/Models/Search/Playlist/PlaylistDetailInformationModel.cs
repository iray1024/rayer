using Rayer.SearchEngine.Netease.Models.Login.Authority;
using Rayer.SearchEngine.Netease.Models.Search.Audio;

namespace Rayer.SearchEngine.Netease.Models.Search.Playlist;

public class PlaylistDetailInformationModel
{
    public long UserId { get; set; }

    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ProfileModel Creator { get; set; } = null!;

    public long CreateTime { get; set; }

    public long UpdateTime { get; set; }

    [JsonPropertyName("coverImgUrl")]
    public string Cover { get; set; } = string.Empty;

    public int TrackCount { get; set; }

    public long PlayCount { get; set; }

    public SearchAudioDetailInformationModel[] Tracks { get; set; } = [];
}