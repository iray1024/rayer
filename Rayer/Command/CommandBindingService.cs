using CommunityToolkit.Mvvm.Input;
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
using Rayer.Views.Pages;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Rayer.Command;

[Inject<ICommandBinding>]
internal partial class CommandBindingService : ICommandBinding
{
    private readonly IAudioManager _audioManager;
    private readonly IPlaybarService _playbarService;
    private readonly IPlaylistService _playlistService;

    public CommandBindingService(
        IAudioManager audioManager,
        IPlaybarService playbarService,
        IPlaylistService playlistService)
    {
        _audioManager = audioManager;
        _playbarService = playbarService;
        _playlistService = playlistService;
    }

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

            var newPlaylistMenu = new NavigationViewItem(name, typeof(PlaylistPage))
            {
                TargetPageTag = $"_playlist_{playlist.Id}"
            };

            nav.MenuItems.Add(newPlaylistMenu);

            RefreshMenuIcon(playlist.Id, null);
        }
    }

    #region Taskbar Control   
    [RelayCommand]
    private async Task Previous()
    {
        await _playbarService.Previous();
    }

    [RelayCommand]
    private void PlayOrPause()
    {
        _playbarService.PlayOrPause();
    }

    [RelayCommand]
    private async Task Next()
    {
        await _playbarService.Next();
    }
    #endregion

    [RelayCommand]
    private async Task Play(object? sender)
    {
        if (sender is AudioCommandParameter parameter)
        {
            if (!parameter.Audio.IsVirualWebSource)
            {
                await _audioManager.Playback.Play(parameter.Audio);
            }
            else
            {
                if (!_audioManager.Playback.TryGetAudio(parameter.Audio.Id, out var existsAudio))
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

                    _audioManager.Playback.Queue.Add(parameter.Audio);

                    await _audioManager.Playback.Play(parameter.Audio);
                }
                else
                {
                    await _audioManager.Playback.Play(existsAudio);
                }
            }
        }
    }

    [RelayCommand]
    private void AddTo(PlaylistUpdate model)
    {
        _playlistService.AddTo(model.Id, model.Target);

        RefreshMenuIcon(model.Id, model.Target.Cover);
    }

    [RelayCommand]
    private void MoveTo(PlaylistUpdate model)
    {
        if (model.To.HasValue)
        {
            _playlistService.Migrate(model.Id, model.To.Value, model.Target);

            var host = App.GetRequiredService<PlaylistPage>();
            host.ViewModel.Items.Remove(model.Target);

            RefreshMenuIcon(model.Id, null);
            RefreshMenuIcon(model.To.Value, model.Target.Cover);
        }
    }

    [RelayCommand]
    private void DeleteFrom(PlaylistUpdate model)
    {
        _playlistService.RemoveFrom(model.Id, model.Target);

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
                if (_audioManager.Playback.Playing &&
                    _audioManager.Playback.Audio.Equals(parameter.Audio))
                {
                    if (_audioManager.Playback.Queue.Count > 1)
                    {
                        await _audioManager.Playback.Next();
                    }
                    else
                    {
                        _audioManager.Playback.EndPlay();
                    }
                }

                _audioManager.Playback.Queue.Remove(parameter.Audio);
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
            if (_audioManager.Playback.Playing)
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

    private void RefreshMenuIcon(int id, ImageSource? icon)
    {
        var nav = App.GetRequiredService<INavigationService>().GetNavigationControl();
        var menu = nav.MenuItems.Cast<NavigationViewItem>().FirstOrDefault(x => x.TargetPageTag == $"_playlist_{id}");

        if (menu is not null)
        {
            var count = _playlistService.Count(id);

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