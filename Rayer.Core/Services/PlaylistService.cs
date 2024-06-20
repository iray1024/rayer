using Rayer.Core.Abstractions;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Models;
using Rayer.Core.Utils;
using System.IO;

namespace Rayer.Core.Services;

[Inject<IPlaylistService>]
internal class PlaylistService : IPlaylistService
{
    private readonly IPlaylistProvider _provider;

    public PlaylistService(IPlaylistProvider provider)
    {
        _provider = provider;
    }

    public IEnumerable<Playlist> Playlists => _provider.Playlists;

    public void Add(Playlist playlist, bool adjustSort = true)
    {
        if (adjustSort)
        {
            var maxSort = _provider.GetFinallySort();

            playlist.Sort = maxSort;
        }

        _provider.Add(playlist);

        Json<Playlist>.StoreData(Path.Combine(Constants.Paths.PlaylistPath, $"{playlist.Name}.json"), playlist);
    }

    public void AddTo(int id, Audio audio)
    {
        var playlist = _provider.Playlists.FirstOrDefault(x => x.Id == id);

        if (playlist is not null)
        {
            playlist.Audios.Add(audio);

            Json<Playlist>.StoreData(Path.Combine(Constants.Paths.PlaylistPath, $"{playlist.Name}.json"), playlist);
        }
    }

    public void Migrate(int from, int to, Audio audio)
    {
        var playlist = _provider.Playlists.FirstOrDefault(x => x.Id == from);
        var toPlaylist = _provider.Playlists.FirstOrDefault(x => x.Id == to);

        if (playlist is not null && toPlaylist is not null)
        {
            playlist.Audios.Remove(audio);
            toPlaylist.Audios.Add(audio);

            Json<Playlist>.StoreData(Path.Combine(Constants.Paths.PlaylistPath, $"{playlist.Name}.json"), playlist);
            Json<Playlist>.StoreData(Path.Combine(Constants.Paths.PlaylistPath, $"{toPlaylist.Name}.json"), toPlaylist);
        }
    }

    public void RemoveFrom(int id, Audio audio)
    {
        var playlist = _provider.Playlists.FirstOrDefault(x => x.Id == id);

        if (playlist is not null)
        {
            playlist.Audios.Remove(audio);

            Json<Playlist>.StoreData(Path.Combine(Constants.Paths.PlaylistPath, $"{playlist.Name}.json"), playlist);
        }
    }

    public void Remove(int id)
    {
        var playlist = _provider.Playlists.FirstOrDefault(x => x.Id == id);

        if (playlist is not null)
        {
            _provider.Playlists.Remove(playlist);

            File.Delete(Path.Combine(Constants.Paths.PlaylistPath, $"{playlist.Name}.json"));
        }
    }

    public int Count(int id)
    {
        var playlist = _provider.Playlists.FirstOrDefault(x => x.Id == id);

        return playlist is not null ? playlist.Audios.Count : -1;
    }
}