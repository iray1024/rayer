using Rayer.Core.Common;

namespace Rayer.Core.Extensions;

public static class StringExtensions
{
    public static string ToDescription(this LyricSearcher lyricSearcher)
    {
        return lyricSearcher switch
        {
            LyricSearcher.Netease => "网易云音乐",
            LyricSearcher.QQMusic => "QQ音乐",
            LyricSearcher.Kugou => "酷狗音乐",
            _ => throw new NotSupportedException()
        };
    }
}