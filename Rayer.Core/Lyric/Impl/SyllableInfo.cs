using Rayer.Core.Lyric.Abstractions;

namespace Rayer.Core.Lyric.Impl;

internal class SyllableInfo(string text, int startTime, int endTime) : ISyllableInfo
{
    public SyllableInfo()
        : this(string.Empty, 0, 0)
    {

    }

    public string Text { get; set; } = text;

    public int StartTime { get; set; } = startTime;

    public int EndTime { get; set; } = endTime;
}