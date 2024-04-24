using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Search.Playlist;

/// <summary>
/// 歌单详情响应模型
/// </summary>
public class PlaylistDetailModel : ResponseBase
{
    public PlaylistDetailInformationModel Playlist { get; set; } = null!;

    public PrivilegesModel[] Privileges { get; set; } = [];
}