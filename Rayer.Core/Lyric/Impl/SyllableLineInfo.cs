using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Enums;
using Rayer.Core.Lyric.Utils;

namespace Rayer.Core.Lyric.Impl;

public class SyllableLineInfo(IEnumerable<ISyllableInfo> syllables) : ILineInfo
{
    private string? _text = null;

    public SyllableLineInfo()
        : this([])
    {

    }

    public string Text => _text ??= SyllableUtils.GetTextFromSyllableList(Syllables);

    public int? StartTime { get; set; } = syllables.First().StartTime;

    public int? EndTime { get; set; } = syllables.Last().EndTime;

    public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(syllables.Last().EndTime - syllables.First().StartTime);

    public LyricAlignment LyricsAlignment { get; set; } = LyricAlignment.Unspecified;

    public ILineInfo? SubLine { get; set; }

    public List<ISyllableInfo> Syllables { get; set; } = [.. syllables];

    public bool IsSyllable => Syllables is { Count: > 0 };

    public int CompareTo(object? obj)
    {
        return obj is ILineInfo line
            ? StartTime is null || line.StartTime is null ? 0 : StartTime == line.StartTime ? 0 : StartTime < line.StartTime ? -1 : 1
            : 0;
    }
}