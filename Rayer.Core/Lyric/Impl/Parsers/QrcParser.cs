using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Enums;
using Rayer.Core.Lyric.Impl.AdditionalFileInfo;
using Rayer.Core.Lyric.Models;
using Rayer.Core.Lyric.Utils;
using System.Text.RegularExpressions;

namespace Rayer.Core.Lyric.Impl.Parsers;

internal static partial class QrcParser
{
    public static LyricData Parse(string lyrics)
    {
        var lyricsLines = lyrics.Trim().Split('\n').ToList();
        var data = new LyricData
        {
            TrackMetadata = new TrackMetadata(),
            File = new()
            {
                Type = LyricType.Qrc,
                SyncTypes = SyncType.SyllableSynced,
                AdditionalInfo = new GeneralAdditionalInfo()
                {
                    Attributes = [],
                }
            }
        };

        var offset = AttributesUtils.ParseGeneralAttributesToLyricsData(data, lyricsLines);

        var lines = ParseLyrics(lyricsLines, offset);

        data.Lines = lines;
        return data;
    }

    public static List<ILineInfo> ParseLyrics(List<string> lines, int? offset = null)
    {
        var list = new List<SyllableLineInfo>();

        foreach (var line in lines)
        {
            var item = ParseLyricsLine(line);
            if (item != null)
            {
                list.Add(item);
            }
        }

        var returnList = list.Cast<ILineInfo>().ToList();
        if (offset.HasValue && offset.Value != 0)
        {
            OffsetUtils.AddOffset(returnList, offset.Value);
        }

        return returnList;
    }

    public static SyllableLineInfo? ParseLyricsLine(string line)
    {
        if (line.Contains(']'))
        {
            line = line[(line.IndexOf(']') + 1)..];
        }

        List<SyllableInfo> lyricItems = [];
        var matches = ItemMacher().Matches(line);

        foreach (var match in matches.Cast<Match>())
        {
            if (match.Groups.Count == 4)
            {
                var text = match.Groups[1].Value;
                var startTime = int.Parse(match.Groups[2].Value);
                var duration = int.Parse(match.Groups[3].Value);

                var endTime = startTime + duration;

                lyricItems.Add(new() { Text = text, StartTime = startTime, EndTime = endTime });
            }
        }

        return new(lyricItems);
    }

    [GeneratedRegex(@"(.*?)\((\d+),(\d+)\)")]
    private static partial Regex ItemMacher();
}