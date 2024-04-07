using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Models;
using System.Windows.Controls;

namespace Rayer.ViewModels;

public partial class RightPlaybarPanelViewModel : ObservableObject
{
    private readonly IAudioManager _audioManager;

    [ObservableProperty]
    private bool _isFlyoutOpen = false;

    [ObservableProperty]
    private SortableObservableCollection<Audio> _items = default!;

    [ObservableProperty]
    private string _queueCount = string.Empty;

    public RightPlaybarPanelViewModel(IAudioManager audioManager, IContextMenuFactory contextMenuFactory)
    {
        _audioManager = audioManager;

        Items = new SortableObservableCollection<Audio>([], AudioSortComparer.Ascending);

        ContextMenu = contextMenuFactory.CreateContextMenu(ContextMenuScope.PlayQueue);
    }

    public ContextMenu ContextMenu { get; }

    public void OnButtonClick()
    {
        if (!IsFlyoutOpen)
        {
            IsFlyoutOpen = true;
        }
    }

    public async Task Play(Audio audio)
    {
        await _audioManager.Playback.Play(audio);
    }
}