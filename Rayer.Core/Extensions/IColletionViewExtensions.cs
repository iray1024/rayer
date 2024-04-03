using System.ComponentModel;

namespace Rayer.Core.Extensions;

public static class IColletionViewExtensions
{
    public static int IndexOf(this ICollectionView collection, object? target)
    {
        var index = 0;
        foreach (var item in collection)
        {
            if (item.Equals(target))
            {
                return index;
            }

            index++;
        }

        return -1;
    }
}