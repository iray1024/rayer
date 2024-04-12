namespace Rayer.Core.Lyric.Abstractions;

public interface IFullLineInfo : ILineInfo
{
    public Dictionary<string, string> Translations { get; }

    public string? ChineseTranslation
    {
        get => Translations.TryGetValue("zh", out var value) ? value : null;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                Translations.Remove("zh");
            }
            else
            {
                Translations["zh"] = value;
            }
        }
    }

    public string? Pronunciation { get; }
}