using Rayer.Core.Models;

namespace Rayer.Core.Abstractions;

public interface IPlaylistService
{
    IEnumerable<Playlist> Playlists { get; }

    void Add(Playlist playlist, bool adjustSort = true);

    void Remove(int id);

    void Update(int id, string newName);

    void AddTo(int id, Audio audio);

    void RemoveFrom(int id, Audio audio);

    void Migrate(int from, int to, Audio audio);

    int Count(int id);
}