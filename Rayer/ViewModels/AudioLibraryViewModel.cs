using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Models;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Rayer.ViewModels;

public partial class AudioLibraryViewModel : ObservableObject
{
    [ObservableProperty]
    private CollectionViewSource _items = default!;

    public AudioLibraryViewModel(IContextMenuFactory contextMenuFactory)
    {
        Items = new CollectionViewSource();
        Items.SortDescriptions.Add(new SortDescription(nameof(Audio.Title), ListSortDirection.Ascending));

        ContextMenu = contextMenuFactory.CreateContextMenu(ContextMenuScope.Library);
    }

    public ContextMenu ContextMenu { get; }
}