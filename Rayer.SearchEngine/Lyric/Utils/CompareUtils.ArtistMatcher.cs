namespace Rayer.SearchEngine.Lyric.Utils;

internal static partial class CompareUtils
{
    public static ArtistMatchType? CompareArtist(IEnumerable<string>? artist1, IEnumerable<string>? artist2)
    {
        if (artist1 == null || artist2 == null)
        {
            return null;
        }

        var list1 = artist1.ToList();
        var list2 = artist2.ToList();

        // 预处理：转小写 & 中文转换
        for (var i = 0; i < list1.Count; i++)
        {
            list1[i] = list1[i].ToLower().ToSC(true);
        }

        for (var i = 0; i < list2.Count; i++)
        {
            list2[i] = list2[i].ToLower().ToSC(true);
        }

        // 比较匹配数量
        var count = 0;
        foreach (var art in list2)
        {
            if (list1.Contains(art))
            {
                count++;
            }
        }

        return count == list1.Count && list1.Count == list2.Count
            ? ArtistMatchType.Perfect
            : (count + 1 >= list1.Count && list1.Count >= 2) || (list1.Count > 6 && (double)count / list1.Count > 0.8)
            ? ArtistMatchType.VeryHigh
            : count == 1 && list1.Count == 1 && list2.Count == 2
            ? ArtistMatchType.High
            : list1.Count > 5 && (list2[0].Contains("Various") || list2[0].Contains("群星"))
            ? ArtistMatchType.VeryHigh
            : list1.Count > 7 && list2.Count > 7 && (double)count / list1.Count > 0.66
            ? ArtistMatchType.High
            : count >= 2 ? ArtistMatchType.Low : ArtistMatchType.NoMatch;
    }

    public static int GetMatchScore(this ArtistMatchType? matchType)
    {
        return matchType switch
        {
            ArtistMatchType.Perfect => 7,
            ArtistMatchType.VeryHigh => 6,
            ArtistMatchType.High => 4,
            ArtistMatchType.Low => 2,
            ArtistMatchType.NoMatch => 0,
            _ => 0,
        };
    }

    /// <summary>
    /// 艺人匹配程度
    /// </summary>
    public enum ArtistMatchType
    {
        Perfect,
        VeryHigh,
        High,
        Low,
        NoMatch = -1,
    }
}