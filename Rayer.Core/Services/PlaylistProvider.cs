using Rayer.Core.Abstractions;
using Rayer.Core.Models;
using Rayer.Core.Utils;
using Rayer.FrameworkCore.Injection;
using System.IO;

namespace Rayer.Core.Services;

[Inject<IPlaylistProvider>]
internal class PlaylistProvider : IPlaylistProvider
{
    private bool _isInitialized = false;

    public ICollection<Playlist> Playlists { get; } = [];

    public int GetFinallySort()
    {
        return Playlists.Count > 0 ? Playlists.Max(x => x.Sort) + 1 : 0;
    }

    public void Initialize(IAudioManager audioManager)
    {
        if (!_isInitialized)
        {
            InitializePlaylist(audioManager);

            _isInitialized = true;
        }
    }

    void IPlaylistProvider.Add(Playlist playlist)
    {
        var maxId = GetFinallyId();

        playlist.Id = maxId;

        Playlists.Add(playlist);
    }

    private void InitializePlaylist(IAudioManager audioManager)
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

                for (var i = 0; i < model.Audios.Count; i++)
                {
                    var instance = audioManager.Audios.FirstOrDefault(x => x.Path == model.Audios[i].Path);

                    if (instance is not null)
                    {
                        model.Audios[i] = instance;
                    }
                    else
                    {
                        if (!model.Audios[i].IsVirualWebSource)
                        {
                            model.Audios[i].Cover = MediaRecognizer.ExtractCover(model.Audios[i].Path);
                        }
                    }
                }

                Playlists.Add(model);
            }
        }
    }

    private int GetFinallyId()
    {
        return Playlists.Count > 0 ? Playlists.Max(x => x.Id) + 1 : 0;
    }
}