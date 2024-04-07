using System.Collections.ObjectModel;

namespace Rayer.Core.Common;

public class SortableObservableCollection<T> : ObservableCollection<T>
{
    private readonly Func<T, T, bool>? _predicate;
    private readonly IComparer<T>? _comparer;

    public SortableObservableCollection(IEnumerable<T> collection, Func<T, T, bool> predicate)
        : base(collection)
    {
        _predicate = predicate;
    }

    public SortableObservableCollection(IEnumerable<T> collection, IComparer<T> comparer)
        : base(collection)
    {
        _comparer = comparer;
    }

    protected override void InsertItem(int index, T item)
    {
        if (index <= Count)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_predicate?.Invoke(item, this[i]) == true ||
                    ((_comparer?.Compare(item, this[i])) is int val && val > 0))
                {
                    index = i;

                    break;
                }
            }
        }

        base.InsertItem(index, item);
    }

    public void AddRange(IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            Add(item);
        }
    }
}