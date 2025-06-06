﻿using Rayer.Core.Common;
using Rayer.Core.Models;
using Rayer.Core.PlayControl.Abstractions;
using Rayer.FrameworkCore.Injection;

namespace Rayer.Core.PlayControl;

[Inject<IPlayQueueProvider>]
internal class PlayQueueProvider : IPlayQueueProvider
{
    public SortableObservableCollection<Audio> Queue { get; }

    public PlayQueueProvider()
    {
        Queue = new SortableObservableCollection<Audio>([], AudioSortComparer.Ascending);
    }
}