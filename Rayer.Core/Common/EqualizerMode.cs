using System.ComponentModel;

namespace Rayer.Core.Common;

public enum EqualizerMode
{
    [Description("关闭")]
    Close = 0,

    [Description("流行")]
    Pop,

    [Description("舞曲")]
    Dancing,

    [Description("蓝调")]
    Blues,

    [Description("古典")]
    Classical,

    [Description("爵士")]
    Jazz,

    [Description("慢歌")]
    Slow,

    [Description("电子乐")]
    Electronica,

    [Description("摇滚")]
    Rock,

    [Description("乡村")]
    Country,

    [Description("人声")]
    Vocal,

    [Description("自定义")]
    Custom
}