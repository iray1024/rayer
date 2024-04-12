using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Impl;

namespace Rayer.Core.Lyric.Utils;

internal static class SyllableUtils
{
    public static string GetTextFromSyllableList(List<ISyllableInfo> syllableList) => string.Concat(syllableList.Select(t => t.Text).ToArray());

    public static string GetTextFromSyllableList(List<SyllableInfo> syllableList) => string.Concat(syllableList.Select(t => t.Text).ToArray());

    public static string GetTextFromSyllableList(List<FullSyllableInfo> syllableList) => string.Concat(syllableList.Select(t => t.Text).ToArray());
}