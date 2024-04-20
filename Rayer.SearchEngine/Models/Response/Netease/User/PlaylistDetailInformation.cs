using Rayer.SearchEngine.Models.Response.Netease.Login.User;
using Rayer.SearchEngine.Models.Response.Netease.Search;

namespace Rayer.SearchEngine.Models.Response.Netease.User;

public class PlaylistDetailInformation
{
    public long UserId { get; set; }

    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ProfileDetail Creator { get; set; } = null!;

    [JsonPropertyName("coverImgUrl")]
    public string Cover { get; set; } = string.Empty;

    public int TrackCount { get; set; }

    public SearchAudioDetailInformation[] Tracks { get; set; } = [];
}