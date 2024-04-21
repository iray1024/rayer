using System.ComponentModel;

namespace Rayer.SearchEngine.Core.Enums;

public enum SearchType
{
    [Description("歌曲")]
    Audio,

    [Description("艺人")]
    Artist,

    [Description("专辑")]
    Album,

    [Description("视频")]
    Video,

    [Description("歌单")]
    Playlist
}