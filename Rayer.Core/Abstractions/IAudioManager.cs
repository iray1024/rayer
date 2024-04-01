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

    void Switch(Audio newAudio);
    void Stop();

    event AudioChangedEventHandler? AudioChanged;
    event EventHandler? AudioStopped;
}

public delegate void AudioChangedEventHandler(object? sender, AudioChangedArgs e);