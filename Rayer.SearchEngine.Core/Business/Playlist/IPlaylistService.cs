using Rayer.SearchEngine.Core.Domain.Playlist;

namespace Rayer.SearchEngine.Core.Business.Playlist;

public interface IPlaylistService
{
    Task<PlaylistDetail> GetPlaylistDetailAsync(long id);
}