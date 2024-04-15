using System.ComponentModel;

namespace Rayer.SearchEngine.Enums;

internal enum SearchType
{
    [Description("歌曲")]
    Audio,

    [Description("艺人")]
    Singer,

    [Description("专辑")]
    Album,

    [Description("视频")]
    Video,

    [Description("歌单")]
    Playlist
}