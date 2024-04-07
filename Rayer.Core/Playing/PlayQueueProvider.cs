using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Models;

namespace Rayer.Core.Playing;

internal class PlayQueueProvider : IPlayQueueProvider
{
    public SortableObservableCollection<Audio> Queue { get; }

    public PlayQueueProvider()
    {
        Queue = new SortableObservableCollection<Audio>([], AudioSortComparer.Ascending);
    }
}