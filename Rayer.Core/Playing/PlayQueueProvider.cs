using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Models;

namespace Rayer.Core.Playing;

public class PlayQueueProvider : IPlayQueueProvider
{
    public SortableObservableCollection<Audio> Queue { get; }

    public PlayQueueProvider()
    {
        
    }
}