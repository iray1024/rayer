using Rayer.SearchEngine.Models.Response.Netease.User;

namespace Rayer.SearchEngine.Business.User.Abstractions;

public interface IUserService
{
    Task<UserLikelist> GetLikelistAsync(long uid);

    Task<UserPlaylist> GetPlaylistAsync(long uid, int limit = 30, int offset = 0);
}