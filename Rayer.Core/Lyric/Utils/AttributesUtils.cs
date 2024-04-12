using Rayer.Core.Lyric.Data;
using Rayer.Core.Lyric.Impl;
using Rayer.Core.Lyric.Impl.AdditionalFileInfo;

namespace Rayer.Core.Lyric.Utils;

internal static class AttributesUtils
{
    public static int? ParseGeneralAttributesToLyricsData(LyricData data, string input, out int index)
    {
        int? offset = null;
        data.TrackMetadata ??= new TrackMetadata();

        index = 0;
        for (; index < input.Length; index++)
        {
            if (input[index] == '[')
            {
                var endIndex = input.IndexOf('\n', index);
                var infoLine = input[index..endIndex];
                if (IsAttributeLine(infoLine))
                {
                    var attribute = GetAttribute(infoLine);
                    switch (attribute.Key)
                    {
                        case "ar":
                            data.TrackMetadata.Artist = attribute.Value;
                            break;
                        case "al":
                            data.TrackMetadata.Album = attribute.Value;
                            break;
                        case "ti":
                            data.TrackMetadata.Title = attribute.Value;
                            break;
                        case "length":
                            if (int.TryParse(attribute.Value, out var result))
                            {
                                data.TrackMetadata.DurationMs = result;
                            }

                            break;
                        case "offset":
                            try { offset = int.Parse(attribute.Value); } catch { }
                            break;
                    }
                    ((GeneralAdditionalInfo)data.File!.AdditionalInfo!).Attributes!.Add(attribute);

                    index = endIndex;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
        return offset;
    }

    public static int? ParseGeneralAttributesToLyricsData(LyricData data, List<string> lines)
    {
        int? offset = null;
        data.TrackMetadata ??= new TrackMetadata();
        for (var i = 0; i < lines.Count; i++)
        {
            if (IsAttributeLine(lines[i]))
            {
                var attribute = GetAttribute(lines[i]);
                switch (attribute.Key)
                {
                    case "ar":
                        data.TrackMetadata.Artist = attribute.Value;
                        break;
                    case "al":
                        data.TrackMetadata.Album = attribute.Value;
                        break;
                    case "ti":
                        data.TrackMetadata.Title = attribute.Value;
                        break;
                    case "length":
                        if (int.TryParse(attribute.Value, out var result))
                        {
                            data.TrackMetadata.DurationMs = result;
                        }

                        break;
                    case "offset":
                        try { offset = int.Parse(attribute.Value); } catch { }
                        break;
                }
                if (attribute.Key == "hash" && data.File!.AdditionalInfo is KrcAdditionalInfo krcAdditionalInfo)
                {
                    krcAdditionalInfo.Hash = attribute.Value;
                }
                else
                {
                    ((GeneralAdditionalInfo)data.File!.AdditionalInfo!).Attributes!.Add(attribute);
                }

                lines.RemoveAt(i--);
            }
            else
            {
                break;
            }
        }
        return offset;
    }

    public static bool IsAttributeLine(string line)
    {
        line = line.Trim();
        return line.StartsWith('[') && line.EndsWith(']') && line.Contains(':');
    }

    private static KeyValuePair<string, string> GetAttribute(string line)
    {
        line = line.Trim();
        var key = Between(line, "[", ":");
        var value = line[(line.IndexOf(':') + 1)..^1];
        return new KeyValuePair<string, string>(key, value);
    }

    private static string Between(string middle, string left, string right)
    {
        if (middle.Contains(left, StringComparison.CurrentCulture))
        {
            middle = middle[(middle.IndexOf(left) + left.Length)..];

            var _end = middle.IndexOf(right);
            if (_end != -1)
            {
                middle = middle[.._end];
            }

            return middle;
        }
        else
        {
            return string.Empty;
        }
    }
}
