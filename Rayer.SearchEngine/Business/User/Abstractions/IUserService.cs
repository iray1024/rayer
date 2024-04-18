using Rayer.SearchEngine.Models.Response.User;

namespace Rayer.SearchEngine.Business.User.Abstractions;

public interface IUserService
{
    Task<UserLikelistResponse> GetLikelistAsync(long uid);

    Task<UserPlaylistResponse> GetPlaylistAsync(long uid, int limit = 30, int offset = 0);
}