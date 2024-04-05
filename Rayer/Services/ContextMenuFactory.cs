using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using System.Windows.Controls;

namespace Rayer.Services;

internal class ContextMenuFactory : IContextMenuFactory
{
    private readonly ICommandBinding _commandBinding;

    public ContextMenuFactory(ICommandBinding commandBinding)
    {
        _commandBinding = commandBinding;
    }

    public ContextMenu CreateContextMenu(ContextMenuScope scope)
    {
        return scope switch
        {
            ContextMenuScope.Library => CreateLibraryContextMenu(),
            ContextMenuScope.Playlist => CreatePlaylistContextMenu(),
            ContextMenuScope.PlaylistPanel => CreatePlaylistPanelContextMenu(),
            ContextMenuScope.PlayQueue => CreatePlayQueueContextMenu(),
            _ => throw new NotImplementedException(),
        };
    }

    private ContextMenu CreateLibraryContextMenu()
    {
        var menu = new ContextMenu();

        menu.Items.Add(new MenuItem()
        {
            Header = "播放",
            Command = _commandBinding.PlayCommand
        });

        menu.Items.Add(new MenuItem()
        {
            Header = "添加到",
            Command= _commandBinding.AddToCommand
        });

        return menu;
    }

    private ContextMenu CreatePlaylistContextMenu()
    {
        var menu = new ContextMenu();

        menu.Items.Add(new MenuItem()
        {
            Header = "播放",
        });

        menu.Items.Add(new MenuItem()
        {
            Header = "添加到"
        });

        menu.Items.Add(new MenuItem()
        {
            Header = "移动到"
        });

        menu.Items.Add(new MenuItem()
        {
            Header = "删除"
        });

        return menu;
    }

    private ContextMenu CreatePlaylistPanelContextMenu()
    {
        var menu = new ContextMenu();

        menu.Items.Add(new MenuItem()
        {
            Header = "播放",
            Command = _commandBinding.PlayCommand

        });

        menu.Items.Add(new MenuItem()
        {
            Header = "删除"
        });

        return menu;
    }

    private ContextMenu CreatePlayQueueContextMenu()
    {
        var menu = new ContextMenu();

        menu.Items.Add(new MenuItem()
        {
            Header = "播放",
            Command = _commandBinding.PlayCommand
        });

        menu.Items.Add(new MenuItem()
        {
            Header = "添加到",
            Command = _commandBinding.AddToCommand
        });

        menu.Items.Add(new MenuItem()
        {
            Header = "删除",
            Command = _commandBinding.DeleteCommand
        });

        return menu;
    }
}