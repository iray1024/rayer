namespace Rayer.SearchEngine.Models.Response.Netease.User;

/// <summary>
/// 用户歌单响应
/// </summary>
public class UserPlaylist : ResponseBase
{
    public PlaylistDetailInformation[] Playlist { get; set; } = [];

    public bool HasMore { get; set; }

    public string Version { get; set; } = string.Empty;
}