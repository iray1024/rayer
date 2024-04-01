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

    public AudioManager(IServiceProvider serviceProvider)
    {
        _playback = new Playback(this);

        var watcher = serviceProvider.GetRequiredService<IAudioFileWatcher>();

        Audios = watcher.Audios;

        var firstAudio = Audios.FirstOrDefault();

        if (firstAudio is not null)
        {
            Playback.Audio = firstAudio;
        }

        foreach (var item in Audios)
        {
            Playback.Queue.Add(item);
        }
    }

    public ObservableCollection<Audio> Audios { get; }

    public Playback Playback => _playback;

    public ICollection<Playlist> Playlists { get; } = [];

    public event AudioChangedEventHandler? AudioChanged;
    public event EventHandler? AudioStopped;

    public void Switch(Audio newAudio)
    {
        AudioChanged?.Invoke(this, new AudioChangedArgs { New = newAudio });
    }

    public void Stop()
    {
        AudioStopped?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        ((IDisposable)_playback).Dispose();

        GC.SuppressFinalize(this);
    }
}