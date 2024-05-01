using CommunityToolkit.Mvvm.Input;
using Rayer.Command.Parameter;
using Rayer.Controls;
using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Menu;
using Rayer.Core.PlayControl.Abstractions;
using Rayer.SearchEngine.Abstractions;
using Rayer.Views.Pages;
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
    private static async Task AddPlaylist()
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
            await _audioManager.Playback.Play(parameter.Audio);
        }
    }

    [RelayCommand]
    private void AddTo(object? sender)
    {

    }

    [RelayCommand]
    private void MoveTo(object? sender)
    {

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

        await System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
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
}