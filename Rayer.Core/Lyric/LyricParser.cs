using Rayer.Core.Lyric.Data;
using Rayer.Core.Lyric.Enums;
using Rayer.Core.Lyric.Impl.Parsers;
using Rayer.Core.Lyrics.Impl.Parsers;

namespace Rayer.Core.Lyric;

public static class LyricParser
{
    public static LyricData? ParseLyrics(string lyric, LyricRawType lyricRawType)
    {
        return lyricRawType switch
        {
            LyricRawType.Lrc => LrcParser.Parse(lyric),
            LyricRawType.Qrc => QrcParser.Parse(lyric),
            LyricRawType.Krc => KryParser.Parse(lyric),
            LyricRawType.Yrc => YrcParser.Parse(lyric),
            _ => null,
        };
    }
}