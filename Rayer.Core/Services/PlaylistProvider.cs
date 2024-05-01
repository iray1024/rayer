using Rayer.Core.Abstractions;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Models;
using Rayer.Core.Utils;
using System.IO;

namespace Rayer.Core.Services;

[Inject<IPlaylistProvider>]
internal class PlaylistProvider : IPlaylistProvider
{
    private bool _isInitialized = false;

    public ICollection<Playlist> Playlists { get; } = [];

    public int GetFinallySort()
    {
        return Playlists.Max(x => x.Sort) + 1;
    }

    public void Initialize()
    {
        if (!_isInitialized)
        {
            InitializePlaylist();

            _isInitialized = true;
        }
    }

    void IPlaylistProvider.Add(Playlist playlist)
    {
        var maxId = GetFinallyId();

        playlist.Id = maxId;

        Playlists.Add(playlist);
    }

    private void InitializePlaylist()
    {
        if (!Directory.Exists(Constants.Paths.PlaylistPath))
        {
            Directory.CreateDirectory(Constants.Paths.PlaylistPath);
        }
        else
        {
            var playlistFiles = Directory.GetFiles(Constants.Paths.PlaylistPath, "*.json");

            foreach (var file in playlistFiles)
            {
                var model = Json<Playlist>.LoadData(file);

                Playlists.Add(model);
            }
        }
    }

    private int GetFinallyId()
    {
        return Playlists.Max(x => x.Id) + 1;
    }
}