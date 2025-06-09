using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Rayer.Core.Common;
using Rayer.Core.Controls;
using Rayer.Core.Menu;
using Rayer.FrameworkCore.Injection;
using System.Windows.Controls;

namespace Rayer.ViewModels;

[Inject(ServiceLifetime = ServiceLifetime.Scoped)]
public partial class PlaylistPageViewModel : AdaptiveViewModelBase
{
    [ObservableProperty]
    private SortableObservableCollection<Audio> _items = default!;

    [ObservableProperty]
    private string _name = default!;

    public PlaylistPageViewModel(IContextMenuFactory contextMenuFactory)
    {
        Items = new SortableObservableCollection<Audio>([], AudioSortComparer.Ascending);

        ContextMenu = contextMenuFactory.CreateContextMenu(ContextMenuScope.Playlist);
    }

    public int Id { get; set; }

    public ContextMenu ContextMenu { get; }
}