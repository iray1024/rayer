using Rayer.Core.Lyric.Abstractions;

namespace Rayer.Core.Lyric.Impl;

public class FullSyllableLineInfo : SyllableLineInfo, IFullLineInfo
{
    public FullSyllableLineInfo() { }

    public FullSyllableLineInfo(SyllableLineInfo lineInfo)
    {
        LyricsAlignment = lineInfo.LyricsAlignment;
        SubLine = lineInfo.SubLine;
        Syllables = lineInfo.Syllables;
    }

    public FullSyllableLineInfo(SyllableLineInfo lineInfo, string? chineseTranslation = null, string? pronunciation = null) : this(lineInfo)
    {
        if (!string.IsNullOrEmpty(chineseTranslation))
        {
            Translations["zh"] = chineseTranslation;
        }

        if (!string.IsNullOrEmpty(pronunciation))
        {
            Pronunciation = pronunciation;
        }
    }

    public Dictionary<string, string> Translations { get; set; } = [];

    public string? Pronunciation { get; set; }
}