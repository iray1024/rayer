using Rayer.Core.Common;
using Rayer.Core.Models;
using Rayer.Core.PlayControl.Abstractions;

namespace Rayer.Core.PlayControl;

internal class PlayQueueProvider : IPlayQueueProvider
{
    public SortableObservableCollection<Audio> Queue { get; }

    public PlayQueueProvider()
    {
        Queue = new SortableObservableCollection<Audio>([], AudioSortComparer.Ascending);
    }
}