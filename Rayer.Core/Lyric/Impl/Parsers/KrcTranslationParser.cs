using Rayer.Core.Lyric.Models;

namespace Rayer.Core.Lyric.Impl.Parsers;

public static class KrcTranslationParser
{
    public static bool CheckKrcTranslation(string krc)
    {
        if (!krc.Contains("[language:"))
        {
            return false;
        }

        try
        {
            var language = krc[(krc.IndexOf("[language:") + "[language:".Length)..];
            language = language[..language.IndexOf(']')];
            var decode = Encoding.UTF8.GetString(Convert.FromBase64String(language));

            var translation = JsonSerializer.Deserialize<KugouTranslation>(decode);
            if (translation!.Content!.Count > 0)
            {
                return true;
            }
        }
        catch { }

        return false;
    }

    public static List<string>? GetTranslationFromKrc(string krc)
    {
        if (!krc.Contains("[language:"))
        {
            return null;
        }

        var language = krc[(krc.IndexOf("[language:") + "[language:".Length)..];
        language = language[..language.IndexOf(']')];
        var decode = Encoding.UTF8.GetString(Convert.FromBase64String(language));

        var translation = JsonSerializer.Deserialize<KugouTranslation>(decode);

        if (translation == null || translation!.Content == null || translation!.Content!.Count == 0)
        {
            return null;
        }

        try
        {
            var result = new List<string>();
            for (var i = 0; i < translation!.Content![0].LyricContent!.Count; i++)
            {
                result.Add(translation!.Content![0].LyricContent![i]![0]);
            }

            return result;
        }
        catch
        {
            return null;
        }
    }

    public static KugouTranslation? GetTranslationRawFromKrc(string krc)
    {
        if (!krc.Contains("[language:"))
        {
            return null;
        }

        var language = krc[(krc.IndexOf("[language:") + "[language:".Length)..];
        language = language[..language.IndexOf(']')];
        var decode = Encoding.UTF8.GetString(Convert.FromBase64String(language));

        return JsonSerializer.Deserialize<KugouTranslation>(decode);
    }
}