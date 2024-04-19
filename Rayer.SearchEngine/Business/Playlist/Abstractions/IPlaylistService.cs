using Rayer.SearchEngine.Models.Response.User;

namespace Rayer.SearchEngine.Business.Playlist.Abstractions;

public interface IPlaylistService
{
    Task<PlaylistDetail> GetPlaylistDetailAsync(long id);
}