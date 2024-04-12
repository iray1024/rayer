using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Common;
using Rayer.Core.Menu;
using System.Windows.Controls;

namespace Rayer.ViewModels;

public partial class AudioLibraryViewModel : ObservableObject
{
    [ObservableProperty]
    private SortableObservableCollection<Audio> _items = default!;

    public AudioLibraryViewModel(IContextMenuFactory contextMenuFactory)
    {
        Items = new SortableObservableCollection<Audio>([], AudioSortComparer.Ascending);

        ContextMenu = contextMenuFactory.CreateContextMenu(ContextMenuScope.Library);
    }

    public ContextMenu ContextMenu { get; }
}