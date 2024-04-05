using System.Collections.ObjectModel;

namespace Rayer.Core.Common;

public class SortableObservableCollection<T> : ObservableCollection<T>
{
    private readonly Func<T, T, bool> _predicate;

    public SortableObservableCollection(IEnumerable<T> collection, Func<T, T, bool> predicate)
        : base(collection)
    {
        _predicate = predicate;
    }

    protected override void InsertItem(int index, T item)
    {
        if (index <= Count)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_predicate(item, this[i]))
                {
                    index = i;
                    break;
                }
            }
        }

        base.InsertItem(index, item);
    }
}