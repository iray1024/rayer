using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Enums;
using Rayer.Core.Lyric.Utils;

namespace Rayer.Core.Lyric.Impl;

public class SyllableLineInfo(IEnumerable<ISyllableInfo> syllables) : ILineInfo
{
    private string? _text = null;
    private int? _startTime = null;
    private int? _endTime = null;

    public SyllableLineInfo()
        : this([])
    {

    }

    public string Text => _text ??= SyllableUtils.GetTextFromSyllableList(Syllables);

    public int? StartTime => _startTime ??= Syllables.First().StartTime;

    public int? EndTime => _endTime ??= Syllables.Last().EndTime;

    public LyricAlignment LyricsAlignment { get; set; } = LyricAlignment.Unspecified;

    public ILineInfo? SubLine { get; set; }

    public List<ISyllableInfo> Syllables { get; set; } = syllables.ToList();

    public bool IsSyllable => Syllables is { Count: > 0 };

    public int CompareTo(object? obj)
    {
        return obj is ILineInfo line
            ? StartTime is null || line.StartTime is null ? 0 : StartTime == line.StartTime ? 0 : StartTime < line.StartTime ? -1 : 1
            : 0;
    }
}