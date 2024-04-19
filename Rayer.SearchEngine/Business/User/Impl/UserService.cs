using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Business.User.Abstractions;
using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Models.Response.User;

namespace Rayer.SearchEngine.Business.User.Impl;

[Inject<IUserService>]
internal class UserService : SearchEngineBase, IUserService
{
    public UserService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public async Task<UserLikelist> GetLikelistAsync(long uid)
    {
        var result = await Searcher.GetAsync(
            User.GetLikelist()
                .WithParam("uid", uid.ToString())
                .Build());

        var response = result.ToEntity<UserLikelist>();

        return response is not null ? response : default!;
    }

    public async Task<UserPlaylist> GetPlaylistAsync(long uid, int limit = 30, int offset = 0)
    {
        var result = await Searcher.GetAsync(
            User.GetPlaylist()
                .WithParam("uid", uid.ToString())
                .WithParam("limit", limit.ToString())
                .WithParam("offset", offset.ToString())
                .Build());

        var response = result.ToEntity<UserPlaylist>();

        if (response is not null)
        {
            foreach (var item in response.Playlist)
            {
                item.Cover += "?param=512y512";
            }

            return response;
        }

        return default!;
    }
}