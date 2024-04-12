using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Data;
using Rayer.Core.Lyric.Enums;
using Rayer.Core.Lyric.Impl;
using Rayer.Core.Lyric.Impl.AdditionalFileInfo;
using Rayer.Core.Lyric.Impl.Parsers;
using Rayer.Core.Lyric.Utils;
using System.Text.RegularExpressions;

namespace Rayer.Core.Lyrics.Impl.Parsers;

internal static partial class KryParser
{
    public static LyricData Parse(string krc)
    {
        var data = new LyricData
        {
            File = new()
            {
                Type = LyricType.Krc,
                SyncTypes = SyncType.SyllableSynced,
                AdditionalInfo = new KrcAdditionalInfo()
                {
                    Attributes = [],
                },
            },
            TrackMetadata = new TrackMetadata()
        };

        var lyricsLines = GetSplitedKrc(krc).ToList();

        var offset = AttributesUtils.ParseGeneralAttributesToLyricsData(data, lyricsLines);

        var lyrics = ParseLyrics(lyricsLines, offset);
        if (KrcTranslationParser.CheckKrcTranslation(krc))
        {
            var lyricsTrans = KrcTranslationParser.GetTranslationFromKrc(krc);
            if (lyricsTrans != null)
            {
                for (var i = 0; i < lyrics.Count && i < lyricsTrans.Count; i++)
                {
                    var t = lyricsTrans[i];
                    t = t != "//" ? t : "";
                    lyrics[i] = new FullSyllableLineInfo((SyllableLineInfo)lyrics[i], chineseTranslation: t);
                }
            }
        }

        data.Lines = lyrics;

        return data;
    }

    public static List<ILineInfo> ParseLyrics(string krc)
    {
        var lyricsLines = GetSplitedKrcWithoutInfoLine(krc).ToList();
        var lyrics = ParseLyrics(lyricsLines);

        if (KrcTranslationParser.CheckKrcTranslation(krc))
        {
            var lyricsTrans = KrcTranslationParser.GetTranslationFromKrc(krc);
            if (lyricsTrans != null)
            {
                for (var i = 0; i < lyrics.Count && i < lyricsTrans.Count; i++)
                {
                    var t = lyricsTrans[i];
                    t = t != "//" ? t : "";
                    lyrics[i] = new FullSyllableLineInfo((SyllableLineInfo)lyrics[i], chineseTranslation: t);
                }
            }
        }

        return lyrics;
    }

    public static List<ILineInfo> ParseLyrics(List<string> lyricsLines, int? offset = null)
    {
        var lyrics = new List<ILineInfo>();

        foreach (var line in lyricsLines)
        {
            if (line.StartsWith('['))
            {
                var l = ParseLyricsLine(line);
                if (l != null)
                {
                    lyrics.Add(l);
                }
            }
        }

        if (offset.HasValue && offset != 0)
        {
            OffsetUtils.AddOffset(lyrics, offset.Value);
        }

        return lyrics;
    }

    private static string[] GetSplitedKrc(string krc)
    {
        var lines = krc
            .Replace("\r\n", "\n")
            .Replace("\r", "")
            .Split('\n');
        return lines;
    }

    private static string[] GetSplitedKrcWithoutInfoLine(string krc)
    {
        var lines = krc
            .Replace("\r\n", "\n")
            .Replace("\r", "")
            .Split('\n');
        var stringBuilder = new StringBuilder();
        foreach (var line in lines)
        {
            if (line.StartsWith('[') && line.Length >= 5 && IsNumber(line[1].ToString()))
            {
                stringBuilder.AppendLine(line);
            }
        }
        return stringBuilder.ToString()
            .Replace("\r\n", "\n")
            .Replace("\r", "")
            .Split('\n');
    }

    public static SyllableLineInfo? ParseLyricsLine(string line)
    {
        var words = line[(line.IndexOf(']') + 1)..].Split(",0>");
        if (words.Length < 1)
        {
            return null;
        }

        var lineTime = line[1..line.IndexOf(']')].Split(',');
        var lineStart = int.Parse(lineTime[0]);

        var syllables = new List<ISyllableInfo>();

        var time = words[0][1..].Split(',');
        var start = int.Parse(time[0]);
        var duration = int.Parse(time[1]);
        for (var i = 1; i < words.Length; i++)
        {
            var word = words[i];
            if (word.Contains('<'))
            {
                word = word[..word.LastIndexOf('<')];
            }
            syllables.Add(new SyllableInfo()
            {
                StartTime = lineStart + start,
                EndTime = lineStart + start + duration,
                Text = word,
            });
            if (words[i].Contains('<'))
            {
                time = words[i][(words[i].LastIndexOf('<') + 1)..].Split(',');
                start = int.Parse(time[0]);
                duration = int.Parse(time[1]);
            }
        }

        return new(syllables);
    }

    private static bool IsNumber(string val)
    {
        return NumberMatcher().IsMatch(val);
    }

    [GeneratedRegex("^\\d+$", RegexOptions.Compiled)]
    private static partial Regex NumberMatcher();
}