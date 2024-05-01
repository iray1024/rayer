using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Common;
using Rayer.Core.Controls;
using Rayer.Core.Menu;
using System.Windows.Controls;

namespace Rayer.ViewModels;

public partial class PlaylistPageViewModel : AdaptiveViewModelBase
{
    [ObservableProperty]
    private SortableObservableCollection<Audio> _items = default!;

    public PlaylistPageViewModel(IContextMenuFactory contextMenuFactory)
    {
        Items = new SortableObservableCollection<Audio>([], AudioSortComparer.Ascending);

        ContextMenu = contextMenuFactory.CreateContextMenu(ContextMenuScope.Playlist);
    }

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ContextMenu ContextMenu { get; }
}