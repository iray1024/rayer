using NAudio.Wave;
using Rayer.Core.Events;
using Rayer.Core.Models;
using Rayer.Core.Playing;
using System.Collections.ObjectModel;

namespace Rayer.Core.Abstractions;

public interface IAudioManager
{
    Playback Playback { get; }

    ICollection<Playlist> Playlists { get; }

    ObservableCollection<Audio> Audios { get; }

    void OnPlaying(PlaybackState oldState);
    void OnPaused();
    void OnSwitch(Audio newAudio);
    void OnStopped();

    event AudioPlayingEventHandler? Playing;
    event EventHandler? Paused;
    event AudioChangedEventHandler? AudioChanged;
    event EventHandler? AudioStopped;
}