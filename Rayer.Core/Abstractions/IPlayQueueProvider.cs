using Rayer.Core.Common;
using Rayer.Core.Models;

namespace Rayer.Core.Abstractions;

public interface IPlayQueueProvider
{
    public SortableObservableCollection<Audio> Queue { get; }
}