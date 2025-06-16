using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Utils;

namespace Rayer.Core.Lyric.Impl;

internal class FullSyllableInfo(IEnumerable<SyllableInfo> syllableInfos) : ISyllableInfo
{
    private string? _text = null;
    private int? _startTime = null;
    private int? _endTime = null;

    public FullSyllableInfo()
        : this([])
    {

    }

    public string Text => _text ??= SyllableUtils.GetTextFromSyllableList(SubItems);

    public int StartTime { get => _startTime ??= SubItems.First().StartTime; set => _startTime = value; }

    public int EndTime { get => _endTime ??= SubItems.Last().EndTime; set => _endTime = value; }

    public List<SyllableInfo> SubItems { get; set; } = [.. syllableInfos];

    public void RefreshProperties()
    {
        _text = null;
        _startTime = null;
        _endTime = null;
    }
}