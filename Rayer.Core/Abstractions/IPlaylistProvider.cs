using Rayer.Core.Models;

namespace Rayer.Core.Abstractions;

internal interface IPlaylistProvider
{
    ICollection<Playlist> Playlists { get; }

    void Add(Playlist playlist);

    int GetFinallySort();

    void Initialize(IAudioManager audioManager);
}