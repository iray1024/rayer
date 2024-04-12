namespace Rayer.SearchEngine.Lyric.Utils;

internal static partial class CompareUtils
{
    public static DurationMatchType? CompareDuration(int? duration1, int? duration2)
    {
        return duration1 == null || duration2 == null
            ? null
            : Math.Abs(duration1.Value - duration2.Value) switch
            {
                0 => DurationMatchType.Perfect,
                < 300 => DurationMatchType.VeryHigh,
                < 700 => DurationMatchType.High,
                < 1500 => DurationMatchType.Medium,
                < 3500 => DurationMatchType.Low,
                _ => DurationMatchType.NoMatch,
            };
    }

    public static int GetMatchScore(this DurationMatchType? matchType)
    {
        return matchType switch
        {
            DurationMatchType.Perfect => 7,
            DurationMatchType.VeryHigh => 6,
            DurationMatchType.High => 5,
            DurationMatchType.Medium => 4,
            DurationMatchType.Low => 2,
            DurationMatchType.NoMatch => 0,
            _ => 0,
        };
    }

    /// <summary>
    /// 时长匹配程度
    /// </summary>
    public enum DurationMatchType
    {
        Perfect,
        VeryHigh,
        High,
        Medium,
        Low,
        NoMatch = -1,
    }
}