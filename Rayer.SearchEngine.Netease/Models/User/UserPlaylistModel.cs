using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Netease.Models.Search.Playlist;

namespace Rayer.SearchEngine.Netease.Models.User;

/// <summary>
/// 用户歌单响应
/// </summary>
public class UserPlaylistModel : ResponseBase
{
    public PlaylistDetailInformationModel[] Playlist { get; set; } = [];

    public bool HasMore { get; set; }

    public string Version { get; set; } = string.Empty;
}