﻿using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Enums;
using Rayer.Core.Lyric.Models;

namespace Rayer.Core.Lyric.Impl.Parsers;

internal static class YrcParser
{
    public static LyricData Parse(string input)
    {
        var lyricsData = new LyricData()
        {
            File = new()
            {
                Type = LyricType.Yrc,
                SyncTypes = SyncType.SyllableSynced,
            },
        };
        var lines = new List<ILineInfo>();

        var i = 0;
        for (; i < input.Length; i++)
        {
            if (input[i] == '{')
            {
                var endIndex = input.IndexOf('\n', i);
                var jsonLine = input[i..endIndex];
                var credits = JsonSerializer.Deserialize<CreditsInfo>(jsonLine);
                if (credits != null)
                {
                    lines.Add(new LineInfo(string.Concat(credits.Credits.Select(c => c.Text)), credits.Timestamp));
                    lyricsData.File.SyncTypes = SyncType.MixedSynced;

                    if (credits.Credits is { Count: > 0 } && credits.Credits[0].Text.StartsWith("作词"))
                    {
                        lyricsData.Writers = credits.Credits
                            .GetRange(1, credits.Credits.Count - 1)
                            .Select(c => c.Text)
                            .Where(t => t != "/")
                            .ToList();
                    }
                }
                i = endIndex;
            }
            else if (input[i] is '\n' or '\r')
            {
                continue;
            }
            else
            {
                break;
            }
        }

        var j = input.Length - 1;
        while (input[j] is '\n' or '\r')
        {
            j--;
        }

        var endCredits = new List<ILineInfo>();
        for (; j >= 0; j--)
        {
            if (input[j] == '}')
            {
                var endIndex = input.LastIndexOf('\n', j);
                var jsonLine = input[(endIndex + 1)..(j + 1)];
                var credits = JsonSerializer.Deserialize<CreditsInfo>(jsonLine);
                if (credits != null)
                {
                    endCredits.Add(new LineInfo(string.Concat(credits.Credits.Select(c => c.Text)), credits.Timestamp));
                    lyricsData.File.SyncTypes = SyncType.MixedSynced;

                    if (credits.Credits is { Count: > 0 } && credits.Credits[0].Text.StartsWith("作词"))
                    {
                        lyricsData.Writers = credits.Credits
                            .GetRange(1, credits.Credits.Count - 1)
                            .Select(c => c.Text)
                            .Where(t => t != "/")
                            .ToList();
                    }
                }
                j = endIndex;
            }
            else if (input[j] is '\n' or '\r')
            {
                continue;
            }
            else
            {
                break;
            }
        }

        var lyricsList = ParseOnlyLyrics(input, i);

        lines.AddRange(lyricsList);
        endCredits.Reverse();
        lines.AddRange(endCredits);

        lyricsData.Lines = lines;

        return lyricsData;
    }

    public static List<ILineInfo> ParseLyrics(string input)
    {
        var lines = new List<ILineInfo>();

        var i = 0;
        for (; i < input.Length; i++)
        {
            if (input[i] == '{')
            {
                var endIndex = input.IndexOf('\n', i);
                var jsonLine = input[i..endIndex];
                var credits = JsonSerializer.Deserialize<CreditsInfo>(jsonLine);
                if (credits != null)
                {
                    lines.Add(new LineInfo(string.Concat(credits.Credits.Select(c => c.Text)), credits.Timestamp));
                }
                i = endIndex;
            }
            else
            {
                break;
            }
        }

        var lyricsList = ParseOnlyLyrics(input, i);

        lines.AddRange(lyricsList);
        return lines;
    }

    public static List<ILineInfo> ParseOnlyLyrics(ReadOnlySpan<char> input, int? startIndex = null)
    {
        var lines = new List<SyllableLineInfo>();
        var karaokeWordInfos = new List<ISyllableInfo>();
        var timeSpanBuilder = 0;
        var lyricStringBuilder = new StringBuilder();
        var wordTimespan = 0;
        var wordDuration = 0;
        var state = CurrentState.None;
        var reachesEnd = false;
        for (var i = startIndex ?? 0; i < input.Length; i++)
        {
            if (i != input.Length)
            {
                ref readonly var curChar = ref input[i];

                if (curChar == '\n' || curChar == '\r' || i + 1 == input.Length)
                {
                    if (i + 1 < input.Length)
                    {
                        if (input[i + 1] is '\n' or '\r')
                        {
                            i++;
                        }

                        karaokeWordInfos.Add(new SyllableInfo(lyricStringBuilder.ToString(), wordTimespan, wordTimespan + wordDuration));
                        lines.Add(new SyllableLineInfo(karaokeWordInfos));
                        karaokeWordInfos.Clear();
                        lyricStringBuilder.Clear();
                        state = CurrentState.None;
                        continue;
                    }
                    if (i + 1 == input.Length)
                    {
                        reachesEnd = true;
                    }
                }
                switch (curChar)
                {
                    case '[':
                        if (state == CurrentState.Lyric)
                        {
                            if (i + 1 < input.Length)
                            {
                                if (!char.IsNumber(input[i + 1]))
                                {
                                    break;
                                }
                            }
                        }
                        state = CurrentState.PossiblyLyricTimestamp;
                        continue;
                    case ',':
                        if (state == CurrentState.Lyric)
                        {
                            if (i + 1 < input.Length)
                            {
                                if (!char.IsNumber(input[i + 1]))
                                {
                                    break;
                                }
                            }
                        }
                        if (state == CurrentState.LyricTimestamp)
                        {
                            state = CurrentState.PossiblyLyricDuration;
                            timeSpanBuilder = 0;
                        }
                        else if (state == CurrentState.WordTimestamp)
                        {
                            state = CurrentState.PossiblyWordDuration;
                            wordTimespan = timeSpanBuilder;
                            timeSpanBuilder = 0;
                        }
                        else
                        {
                            state = CurrentState.WordUnknownItem;
                            wordDuration = timeSpanBuilder;
                            timeSpanBuilder = 0;
                        }
                        continue;
                    case ']':
                        if (state == CurrentState.Lyric)
                        {
                            if (i + 1 < input.Length)
                            {
                                if (!char.IsNumber(input[i + 1]))
                                {
                                    break;
                                }
                            }
                        }
                        state = CurrentState.None;
                        timeSpanBuilder = 0;
                        continue;
                    case '(':
                        if (state == CurrentState.Lyric)
                        {
                            if (i + 1 < input.Length)
                            {
                                if (!char.IsNumber(input[i + 1]))
                                {
                                    break;
                                }
                            }
                            karaokeWordInfos.Add(new SyllableInfo(lyricStringBuilder.ToString(), wordTimespan, wordTimespan + wordDuration));
                            lyricStringBuilder.Clear();
                        }
                        state = CurrentState.PossiblyWordTimestamp;
                        continue;
                    case ')':
                        if (state == CurrentState.Lyric)
                        {
                            if (i + 1 < input.Length)
                            {
                                if (!char.IsNumber(input[i + 1]))
                                {
                                    break;
                                }
                            }
                        }
                        state = CurrentState.Lyric;
                        continue;
                }
                switch (state)
                {
                    case CurrentState.PossiblyLyricTimestamp:
                        if (char.IsNumber(curChar))
                        {
                            state = CurrentState.LyricTimestamp;
                        }

                        timeSpanBuilder *= 10;
                        timeSpanBuilder += curChar - '0';
                        break;
                    case CurrentState.LyricTimestamp:
                        timeSpanBuilder *= 10;
                        timeSpanBuilder += curChar - '0';
                        break;
                    case CurrentState.PossiblyWordTimestamp:
                        if (char.IsNumber(curChar))
                        {
                            state = CurrentState.WordTimestamp;
                        }

                        timeSpanBuilder *= 10;
                        timeSpanBuilder += curChar - '0';
                        break;
                    case CurrentState.WordTimestamp:
                        timeSpanBuilder *= 10;
                        timeSpanBuilder += curChar - '0';
                        break;
                    case CurrentState.PossiblyLyricDuration:
                        if (char.IsNumber(curChar))
                        {
                            state = CurrentState.LyricDuration;
                        }

                        timeSpanBuilder *= 10;
                        timeSpanBuilder += curChar - '0';
                        break;
                    case CurrentState.LyricDuration:
                        timeSpanBuilder *= 10;
                        timeSpanBuilder += curChar - '0';
                        break;
                    case CurrentState.PossiblyWordDuration:
                        if (char.IsNumber(curChar))
                        {
                            state = CurrentState.WordDuration;
                        }

                        timeSpanBuilder *= 10;
                        timeSpanBuilder += curChar - '0';
                        break;
                    case CurrentState.WordDuration:
                        timeSpanBuilder *= 10;
                        timeSpanBuilder += curChar - '0';
                        break;
                    case CurrentState.Lyric:
                        if (reachesEnd && (input[i] == '\n' || input[i] == '\r'))
                        {
                            break;
                        }

                        lyricStringBuilder.Append(curChar);
                        break;
                }
                if (reachesEnd)
                {
                    karaokeWordInfos.Add(new SyllableInfo(lyricStringBuilder.ToString(), wordTimespan, wordTimespan + wordDuration));
                    lines.Add(new SyllableLineInfo(karaokeWordInfos));
                    karaokeWordInfos.Clear();
                    lyricStringBuilder.Clear();
                }
            }
        }
        return lines.Cast<ILineInfo>().ToList();
    }

    private enum CurrentState
    {
        None,
        LyricTimestamp,
        WordTimestamp,
        LyricDuration,
        WordDuration,
        WordUnknownItem,
        PossiblyLyricDuration,
        PossiblyWordDuration,
        PossiblyLyricTimestamp,
        PossiblyWordTimestamp,
        Lyric
    }
}