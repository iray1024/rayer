using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Enums;

namespace Rayer.Core.Lyric.Impl;

public class LineInfo(string text, int? startTime) : ILineInfo
{
    public LineInfo()
        : this(string.Empty)
    {

    }

    public LineInfo(string text)
        : this(text, null)
    {

    }

    public LineInfo(string text, int startTime, int? endTime) : this(text, startTime)
    {
        EndTime = endTime;
    }

    public string Text { get; set; } = text;

    public int? StartTime { get; set; } = startTime;

    public int? EndTime { get; set; }
    public LyricAlignment LyricsAlignment { get; set; } = LyricAlignment.Unspecified;
    public ILineInfo? SubLine { get; set; }

    public int CompareTo(object? obj)
    {
        return obj is ILineInfo line
            ? StartTime is null || line.StartTime is null ? 0 : StartTime == line.StartTime ? 0 : StartTime < line.StartTime ? -1 : 1
            : 0;
    }
}