using Rayer.Core.Common;
using Rayer.Core.Models;

namespace Rayer.Core.PlayControl.Abstractions;

public interface IPlayQueueProvider
{
    public SortableObservableCollection<Audio> Queue { get; }
}