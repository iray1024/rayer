using Rayer.SearchEngine.Models.Response.Netease;

namespace Rayer.SearchEngine.Models.Response.Netease.User;

/// <summary>
/// 歌单详情响应模型
/// </summary>
public class PlaylistDetail : ResponseBase
{
    public PlaylistDetailInformation Playlist { get; set; } = null!;

    public Privileges[] Privileges { get; set; } = [];
}