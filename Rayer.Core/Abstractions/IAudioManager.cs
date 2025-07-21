using Rayer.Core.Events;
using Rayer.Core.Models;
using Rayer.Core.Playing;
using System.Collections.ObjectModel;

namespace Rayer.Core.Abstractions;

public interface IAudioManager
{
    Playback Playback { get; }

    IEnumerable<Playlist> Playlists { get; }

    ObservableCollection<Audio> Audios { get; }

    event EventHandler<AudioPlayingArgs>? AudioPlaying;
    event EventHandler? AudioPaused;
    event EventHandler<AudioChangedArgs>? AudioChanged;
    event EventHandler? AudioStopped;
    event EventHandler? PreLoaded;
}