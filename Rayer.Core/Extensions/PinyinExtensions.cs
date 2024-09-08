using hyjiacan.py4n;

namespace Rayer.Core.Extensions;

public static class PinyinExtensions
{
    public static IEnumerable<string> Combine(this IEnumerable<PinyinItem> source)
    {
        var result = new List<string>();

        var firstNode = source.First();

        if (source.Count() > 1)
        {
            foreach (var sectionItem in source.Skip(1).Combine())
            {
                foreach (var item in firstNode)
                {
                    result.Add($"{item}{sectionItem}");
                }
            }
        }
        else
        {
            foreach (var item in firstNode)
            {
                result.Add(item);
            }
        }

        return result.Select(x => x.ToString());
    }
}