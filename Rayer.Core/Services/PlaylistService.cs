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
        throw new NotImplementedException();
    }

    public void Migrate(int from, int to, Audio audio)
    {
        throw new NotImplementedException();
    }

    public void Remove(int id)
    {
        throw new NotImplementedException();
    }

    public void RemoveFrom(int id, Audio audio)
    {
        throw new NotImplementedException();
    }
}