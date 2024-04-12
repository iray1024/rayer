namespace Rayer.Core.Lyric.Utils;

internal static class MathUtils
{
    public static int? Min(int? left, int? right)
    {
        return left.HasValue && right.HasValue
            ? Math.Min(left.Value, right.Value)
            : left.HasValue ? left.Value : right.HasValue ? right.Value : null;
    }

    public static int? Max(int? left, int? right)
    {
        return left.HasValue && right.HasValue
            ? Math.Max(left.Value, right.Value)
            : left.HasValue ? left.Value : right.HasValue ? right.Value : null;
    }
}