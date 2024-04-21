using Rayer.SearchEngine.Core.Domain.Playlist;

namespace Rayer.SearchEngine.Core.Business.User;

public interface IUserService
{
    Task<PlaylistDetail> GetLikelistAsync(long uid);

    Task<PlaylistDetail[]> GetPlaylistAsync(long uid, int limit = 30, int offset = 0);
}