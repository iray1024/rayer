using Microsoft.Extensions.DependencyInjection;
using Rayer.Core.Abstractions;
using Rayer.Core.Events;
using Rayer.Core.FileSystem.Abstractions;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Models;
using Rayer.Core.Playing;
using System.Collections.ObjectModel;

namespace Rayer.Core.Services;

[Inject<IAudioManager>]
internal class AudioManager : IAudioManager, IDisposable
{
    private readonly IAudioFileWatcher _audioFileWatcher;
    private readonly IPlaylistProvider _playlistProvider;

    public AudioManager(IServiceProvider serviceProvider)
    {
        _audioFileWatcher = serviceProvider.GetRequiredService<IAudioFileWatcher>();
        _playlistProvider = serviceProvider.GetRequiredService<IPlaylistProvider>();

        var settings = serviceProvider.GetRequiredService<ISettingsService>().Settings;

        Playback = new Playback(this, serviceProvider);

        Playback.Initialize(settings.Volume, settings.Pitch, settings.PlayloopMode);

        Playback.AudioPlaying += OnAudioPlaying;
        Playback.AudioPaused += OnAudioPaused;
        Playback.AudioChanged += OnAudioChanged;
        Playback.AudioStopped += OnAudioStopped;
    }

    public ObservableCollection<Audio> Audios => _audioFileWatcher.Audios;

    public Playback Playback { get; }

    public IEnumerable<Playlist> Playlists => _playlistProvider.Playlists;

    public event EventHandler<AudioPlayingArgs>? AudioPlaying;
    public event EventHandler? AudioPaused;
    public event EventHandler<AudioChangedArgs>? AudioChanged;
    public event EventHandler? AudioStopped;

    protected virtual void OnAudioPlaying(object? sender, AudioPlayingArgs e)
    {
        AudioPlaying?.Invoke(this, new AudioPlayingArgs { PlaybackState = e.PlaybackState });
    }

    protected virtual void OnAudioPaused(object? sender, EventArgs e)
    {
        AudioPaused?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnAudioChanged(object? sender, AudioChangedArgs e)
    {
        AudioChanged?.Invoke(this, new AudioChangedArgs { New = e.New });
    }

    protected virtual void OnAudioStopped(object? sender, EventArgs e)
    {
        AudioStopped?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        ((IDisposable)Playback).Dispose();

        GC.SuppressFinalize(this);
    }
}