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

    event AudioPlayingEventHandler? AudioPlaying;
    event EventHandler? AudioPaused;
    event AudioChangedEventHandler? AudioChanged;
    event EventHandler? AudioStopped;
}