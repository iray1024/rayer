using Rayer.SearchEngine.Netease.Models.Login.Authority;
using Rayer.SearchEngine.Netease.Models.Search;

namespace Rayer.SearchEngine.Netease.Models.User;

public class PlaylistDetailInformationModel
{
    public long UserId { get; set; }

    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ProfileModel Creator { get; set; } = null!;

    [JsonPropertyName("coverImgUrl")]
    public string Cover { get; set; } = string.Empty;

    public int TrackCount { get; set; }

    public SearchAudioDetailInformationModel[] Tracks { get; set; } = [];
}