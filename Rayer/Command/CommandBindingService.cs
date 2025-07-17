using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Rayer.Controls;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Menu;
using Rayer.Core.PlayControl.Abstractions;
using Rayer.Core.Utils;
using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.Services;
using Rayer.ViewModels;
using Rayer.Views.Pages;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace Rayer.Command;

[Inject<ICommandBinding>]
internal partial class CommandBindingService(
    IAudioManager audioManager,
    IPlaybarService playbarService,
    IPlaylistService playlistService) : ICommandBinding
{
    [RelayCommand]
    private async Task AddPlaylist()
    {
        var dialogService = App.GetRequiredService<IContentDialogService>();
        var dialog = new NewPlaylistDialog(dialogService.GetDialogHost())
        {
            Title = "新建歌单",
            PrimaryButtonText = "确认",
            CloseButtonText = "取消"
        };

        var result = await dialog.ShowAsync(AppCore.StoppingToken);
        if (result is ContentDialogResult.Primary)
        {
            var name = dialog.PlaylistName.Text;

            var playlistService = App.GetRequiredService<IPlaylistService>();
            var nav = App.GetRequiredService<INavigationService>().GetNavigationControl();

            var playlist = new Playlist()
            {
                Name = name,
            };

            playlistService.Add(playlist);

            var contextMenuFactory = AppCore.GetRequiredService<IContextMenuFactory>();
            var tag = $"_playlist_{playlist.Id}";
            var newPlaylistMenu = new NavigationViewItem(name, typeof(PlaylistPage))
            {
                TargetPageTag = tag,
                ContextMenu = contextMenuFactory.CreateContextMenu(ContextMenuScope.PlaylistMenu, tag),
                Content = new Emoji.Wpf.TextBlock()
                {
                    Text = name
                }
            };

            nav.MenuItems.Add(newPlaylistMenu);

            RefreshMenuIcon(playlist.Id, null);
        }
    }

    [RelayCommand]
    private async Task EditPlaylist(string tag)
    {
        var nav = App.GetRequiredService<INavigationService>().GetNavigationControl();
        var target = nav.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(x => x.TargetPageTag == tag)!;

        var dialogService = App.GetRequiredService<IContentDialogService>();
        var dialog = new NewPlaylistDialog(dialogService.GetDialogHost())
        {
            Title = "编辑歌单",
            PrimaryButtonText = "确认",
            CloseButtonText = "取消"
        };

        dialog.PlaylistName.Text = (target.Content as Emoji.Wpf.TextBlock)?.Text ?? string.Empty;

        var result = await dialog.ShowAsync(AppCore.StoppingToken);
        if (result is ContentDialogResult.Primary)
        {
            var name = dialog.PlaylistName.Text;
            ((Emoji.Wpf.TextBlock)target.Content).Text = name;

            AppCore.GetRequiredService<PlaylistPageViewModel>().Name = name;
            playlistService.Update(int.Parse(tag[10..]), name);
        }
    }

    [RelayCommand]
    private async Task DeletePlaylist(string tag)
    {
        var nav = App.GetRequiredService<INavigationService>().GetNavigationControl();
        var target = nav.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(x => x.TargetPageTag == tag)!;

        var dialogService = App.GetRequiredService<IContentDialogService>();
        var result = await dialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions
        {
            Title = $"Rayer ({target.Content})",
            Content = "确定删除歌单？若由于误操作删除歌单，可没有办法恢复~",
            PrimaryButtonText = "确认",
            CloseButtonText = "取消"
        });

        if (result is ContentDialogResult.Primary)
        {
            playlistService.Remove(int.Parse(tag[10..]));
            nav.MenuItems.Remove(target);
        }
    }

    #region Taskbar Control   
    [RelayCommand]
    private async Task Previous()
    {
        await playbarService.Previous();
    }

    [RelayCommand]
    private void PlayOrPause()
    {
        playbarService.PlayOrPause();
    }

    [RelayCommand]
    private async Task Next()
    {
        await playbarService.Next();
    }
    #endregion

    [RelayCommand]
    private async Task Play(object? sender)
    {
        if (sender is AudioCommandParameter parameter)
        {
            if (!parameter.Audio.IsVirualWebSource)
            {
                await audioManager.Playback.Play(parameter.Audio);
            }
            else
            {
                if (!audioManager.Playback.TryGetAudio(parameter.Audio.Id, out var existsAudio))
                {
                    var provider = App.GetRequiredService<ISearchAudioEngineProvider>();
                    var engine = provider.GetAudioEngine(parameter.Audio.SearcherType);

                    var audio = await engine.GetAudioAsync(new SearchEngine.Core.Domain.Aduio.SearchAudioDetail
                    {
                        Id = long.Parse(parameter.Audio.Id),
                        Tags = parameter.Audio.Tags
                    });

                    parameter.Audio.Path = audio.Url;

                    if (parameter.Audio.Cover is null && parameter.Audio.CoverUri is not null)
                    {
                        parameter.Audio.Cover = await ImageSourceFactory.CreateWebSourceAsync(new Uri(parameter.Audio.CoverUri));
                    }

                    audioManager.Playback.Queue.Add(parameter.Audio);

                    await audioManager.Playback.Play(parameter.Audio);
                }
                else
                {
                    await audioManager.Playback.Play(existsAudio);
                }
            }
        }
    }

    [RelayCommand]
    private void AddTo(PlaylistUpdate model)
    {
        playlistService.AddTo(model.Id, model.Target);

        RefreshMenuIcon(model.Id, model.Target.Cover);
    }

    [RelayCommand]
    private void MoveTo(PlaylistUpdate model)
    {
        if (model.To.HasValue)
        {
            playlistService.Migrate(model.Id, model.To.Value, model.Target);

            var host = App.GetRequiredService<PlaylistPage>();
            host.ViewModel.Items.Remove(model.Target);

            RefreshMenuIcon(model.Id, null);
            RefreshMenuIcon(model.To.Value, model.Target.Cover);
        }
    }

    [RelayCommand]
    private void DeleteFrom(PlaylistUpdate model)
    {
        playlistService.RemoveFrom(model.Id, model.Target);

        var host = App.GetRequiredService<PlaylistPage>();
        host.ViewModel.Items.Remove(model.Target);

        RefreshMenuIcon(model.Id, null);
    }

    [RelayCommand]
    private async Task Delete(object? sender)
    {
        if (sender is AudioCommandParameter parameter)
        {
            if (parameter.Scope is ContextMenuScope.PlayQueue)
            {
                if (audioManager.Playback.Playing &&
                    audioManager.Playback.Audio.Equals(parameter.Audio))
                {
                    if (audioManager.Playback.Queue.Count > 1)
                    {
                        await audioManager.Playback.Next();
                    }
                    else
                    {
                        audioManager.Playback.EndPlay();
                    }
                }

                audioManager.Playback.Queue.Remove(parameter.Audio);
            }
        }
    }

    #region DynamicIsland    
    [RelayCommand]
    private async Task SwitchLyricSearcher(LyricSearcher searcher)
    {
        var settingsService = App.GetRequiredService<ISettingsService>();

        settingsService.Settings.LyricSearcher = searcher;
        settingsService.Save();

        await Task.Run(async () =>
        {
            if (audioManager.Playback.Playing)
            {
                var provider = App.GetRequiredService<ILyricProvider>();
                await provider.SwitchSearcherAsync();
            }
        });
    }

    [RelayCommand]
    private static void FastForward()
    {
        var provider = App.GetRequiredService<ILyricProvider>();

        provider.FastForward();
    }

    [RelayCommand]
    private static void FastBackward()
    {
        var provider = App.GetRequiredService<ILyricProvider>();

        provider.FastBackward();
    }
    #endregion

    [RelayCommand]
    private static async Task SetAlbumCover(AlbumCoverCommandParameter parameter)
    {
        var folderBrowserDialog = new OpenFileDialog
        {
            Title = "选择媒体文件",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Filter = "媒体文件 (*.mp4, *.mkv, *.mov)|*.mp4;*.mkv;*.mov|所有文件 (*.*)|*.*",
            Multiselect = false,
        };

        if (folderBrowserDialog.ShowDialog() == true)
        {
            var path = folderBrowserDialog.FileName;

            var coverManager = AppCore.GetRequiredService<ICoverManager>();
            await coverManager.SetCoverAsync(parameter.Audio, path);

            if (parameter.Context.Items.Cast<System.Windows.Controls.MenuItem>().FirstOrDefault(x => x.Header.Equals("移除封面")) is System.Windows.Controls.MenuItem menuItem)
            {
                menuItem.IsEnabled = true;
            }
        }
    }

    [RelayCommand]
    private static async Task RemoveAlbumCover(AlbumCoverCommandParameter parameter)
    {
        var coverManager = AppCore.GetRequiredService<ICoverManager>();
        await coverManager.RemoveCoverAsync(parameter.Audio);

        if (parameter.Context.Items.Cast<System.Windows.Controls.MenuItem>().FirstOrDefault(x => x.Header.Equals("移除封面")) is System.Windows.Controls.MenuItem menuItem)
        {
            menuItem.IsEnabled = false;
        }
    }

    private void RefreshMenuIcon(int id, ImageSource? icon)
    {
        var nav = App.GetRequiredService<INavigationService>().GetNavigationControl();
        var menu = nav.MenuItems.Cast<NavigationViewItem>().FirstOrDefault(x => x.TargetPageTag == $"_playlist_{id}");

        if (menu is not null)
        {
            var count = playlistService.Count(id);

            if (count == 0)
            {
                menu.Icon = new ImageIcon()
                {
                    Source = StaticThemeResources.AlbumFallback,
                    Width = 24,
                    Height = 24
                };
            }

            if (count == 1)
            {
                if (icon is not null)
                {
                    menu.Icon = new ImageIcon()
                    {
                        Source = icon,
                        Width = 24,
                        Height = 24
                    };
                }
            }

            if (menu.Icon is not null)
            {
                menu.Icon.Clip = new RectangleGeometry(new Rect(0, 0, 24, 24), 4, 4);
            }
        }
    }
}