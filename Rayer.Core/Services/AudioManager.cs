using Microsoft.Extensions.DependencyInjection;
using Rayer.Core.Abstractions;
using Rayer.Core.Events;
using Rayer.Core.Models;
using Rayer.Core.Playing;
using System.Collections.ObjectModel;

namespace Rayer.Core.Services;

internal class AudioManager : IAudioManager, IDisposable
{
    private readonly Playback _playback;
    private readonly IAudioFileWatcher _audioFileWatcher;

    public AudioManager(IServiceProvider serviceProvider)
    {
        _audioFileWatcher = serviceProvider.GetRequiredService<IAudioFileWatcher>();

        var settings = serviceProvider.GetRequiredService<ISettingsService>().Settings;

        _playback = new Playback(this, serviceProvider);

        _playback.Initialize(settings.Volume, settings.Pitch, settings.PlayloopMode);

        _playback.AudioPlaying += OnAudioPlaying;
        _playback.AudioPaused += OnAudioPaused;
        _playback.AudioChanged += OnAudioChanged;
        _playback.AudioStopped += OnAudioStopped;
    }

    public ObservableCollection<Audio> Audios => _audioFileWatcher.Audios;

    public Playback Playback => _playback;

    public ICollection<Playlist> Playlists { get; } = [];

    public event AudioPlayingEventHandler? AudioPlaying;
    public event EventHandler? AudioPaused;
    public event AudioChangedEventHandler? AudioChanged;
    public event EventHandler? AudioStopped;

    public void OnAudioPlaying(object? sender, AudioPlayingArgs e)
    {
        AudioPlaying?.Invoke(this, new AudioPlayingArgs { PlaybackState = e.PlaybackState });
    }

    public void OnAudioPaused(object? sender, EventArgs e)
    {
        AudioPaused?.Invoke(this, EventArgs.Empty);
    }

    public void OnAudioChanged(object? sender, AudioChangedArgs e)
    {
        AudioChanged?.Invoke(this, new AudioChangedArgs { New = e.New });
    }

    public void OnAudioStopped(object? sender, EventArgs e)
    {
        AudioStopped?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        ((IDisposable)_playback).Dispose();

        GC.SuppressFinalize(this);
    }
}