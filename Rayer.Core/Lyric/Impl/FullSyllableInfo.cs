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

    public int StartTime => _startTime ??= SubItems.First().StartTime;

    public int EndTime => _endTime ??= SubItems.Last().EndTime;

    public List<SyllableInfo> SubItems { get; set; } = syllableInfos.ToList();

    public void RefreshProperties()
    {
        _text = null;
        _startTime = null;
        _endTime = null;
    }
}