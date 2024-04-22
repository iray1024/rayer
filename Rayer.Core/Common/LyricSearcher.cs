using System.ComponentModel;

namespace Rayer.Core.Common;

public enum LyricSearcher
{
    [Description("网易云音乐")]
    Netease = 0,

    [Description("QQ音乐")]
    QQMusic,

    [Description("酷狗音乐")]
    Kugou
}