using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Menu;
using Rayer.Core.Utils;
using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using System.Windows.Controls;

namespace Rayer.Services;

[Inject<IContextMenuFactory>]
internal class ContextMenuFactory : IContextMenuFactory
{
    private readonly ICommandBinding _commandBinding;

    public ContextMenuFactory(ICommandBinding commandBinding)
    {
        _commandBinding = commandBinding;
    }

    public ContextMenu CreateContextMenu(ContextMenuScope scope, object? commandParameter = null)
    {
        return scope switch
        {
            ContextMenuScope.Library => CreateLibraryContextMenu(),
            ContextMenuScope.Playlist => CreatePlaylistContextMenu(),
            ContextMenuScope.PlaylistPanel => CreatePlaylistPanelContextMenu(),
            ContextMenuScope.PlaylistMenu => CreatePlaylistMenuContextMenu(commandParameter),
            ContextMenuScope.PlayQueue => CreatePlayQueueContextMenu(),
            ContextMenuScope.DynamicIsland => CreateDynamicIslandContextMenu(),
            ContextMenuScope.AlbumPresenter => CreateAlbumPresenterContextMenu(commandParameter),
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
            Header = "添加到"
        });

        return menu;
    }

    private ContextMenu CreatePlaylistContextMenu()
    {
        var menu = new ContextMenu();

        menu.Items.Add(new MenuItem()
        {
            Header = "播放",
            Command = _commandBinding.PlayCommand
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
            Header = "删除",
            Command = _commandBinding.DeleteFromCommand
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
            Header = "删除",
            Command = _commandBinding.DeleteCommand
        });

        return menu;
    }

    private ContextMenu CreatePlaylistMenuContextMenu(object? commandParameter = null)
    {
        var menu = new ContextMenu();

        menu.Items.Add(new MenuItem()
        {
            Header = "编辑歌单",
            Command = _commandBinding.EditPlaylistCommand,
            CommandParameter = commandParameter
        });

        menu.Items.Add(new MenuItem()
        {
            Header = "删除歌单",
            Command = _commandBinding.DeletePlaylistCommand,
            CommandParameter = commandParameter
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
            Header = "添加到"
        });

        menu.Items.Add(new MenuItem()
        {
            Header = "删除",
            Command = _commandBinding.DeleteCommand
        });

        return menu;
    }

    private ContextMenu CreateDynamicIslandContextMenu()
    {
        var menu = new ContextMenu();

        var switchLyricSearcherItem = new MenuItem()
        {
            Header = "切换歌词搜索器"
        };

        var neteaseSearcher = new MenuItem()
        {
            Header = "网易云音乐",
            Command = _commandBinding.SwitchLyricSearcherCommand,
            CommandParameter = LyricSearcher.Netease
        };
        var qqSearcher = new MenuItem()
        {
            Header = "QQ音乐",
            Command = _commandBinding.SwitchLyricSearcherCommand,
            CommandParameter = LyricSearcher.QQMusic
        };
        var kugouSearcher = new MenuItem()
        {
            Header = "酷狗音乐",
            Command = _commandBinding.SwitchLyricSearcherCommand,
            CommandParameter = LyricSearcher.Kugou
        };

        neteaseSearcher.Click += OnSearcherChecked;
        qqSearcher.Click += OnSearcherChecked;
        kugouSearcher.Click += OnSearcherChecked;

        switchLyricSearcherItem.Items.Add(neteaseSearcher);
        switchLyricSearcherItem.Items.Add(qqSearcher);
        switchLyricSearcherItem.Items.Add(kugouSearcher);

        menu.Items.Add(switchLyricSearcherItem);

        menu.Items.Add(new Separator());

        var fastbackward = new MenuItem()
        {
            Header = "快退0.5秒",
            Command = _commandBinding.FastBackwardCommand,
            StaysOpenOnClick = true
        };

        var fastforward = new MenuItem()
        {
            Header = "快进0.5秒",
            Command = _commandBinding.FastForwardCommand,
            StaysOpenOnClick = true
        };

        menu.Items.Add(fastbackward);
        menu.Items.Add(fastforward);

        return menu;
    }

    private ContextMenu CreateAlbumPresenterContextMenu(object? commandParameter = null)
    {
        var menu = new ContextMenu();

        if (commandParameter is Audio audio)
        {
            var parameter = new AlbumCoverCommandParameter(audio, menu);

            menu.Items.Add(new MenuItem()
            {
                Header = "设置封面",
                Icon = ImageIconFactory.Create("Media", 18),
                Command = _commandBinding.SetAlbumCoverCommand,
                CommandParameter = parameter
            });

            var coverManager = AppCore.GetRequiredService<ICoverManager>();
            menu.Items.Add(new MenuItem()
            {
                Header = "移除封面",
                Icon = ImageIconFactory.Create("Recycle", 18),
                Command = _commandBinding.RemoveAlbumCoverCommand,
                CommandParameter = parameter,
                IsEnabled = coverManager.GetCover(audio) is not null
            });
        }

        return menu;
    }

    private void OnSearcherChecked(object sender, System.Windows.RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem)
        {
            var parent = (MenuItem)menuItem.Parent;

            foreach (var item in parent.Items.Cast<MenuItem>())
            {
                item.Icon = null;
            }

            menuItem.Icon = ImageIconFactory.Create("Play", 18);
        }
    }
}