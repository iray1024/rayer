using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Business.User;
using Rayer.SearchEngine.Core.Domain.Playlist;
using Rayer.SearchEngine.Core.Enums;
using Rayer.SearchEngine.Netease.Engine;
using Rayer.SearchEngine.Netease.Models.User;

namespace Rayer.SearchEngine.Netease.Business.User;

[Inject<IUserService>(ServiceKey = SearcherType.Netease)]
internal class UserService : SearchEngineBase, IUserService
{
    public UserService()
    {
    }

    public async Task<PlaylistDetail> GetLikelistAsync(long uid)
    {
        var result = await Searcher.GetAsync(
            UserSelector.GetLikelist()
                .WithParam("uid", uid.ToString())
                .Build());

        var response = result.ToEntity<UserLikelistModel>();

        if (response is not null)
        {
            var domain = Mapper.Map<PlaylistDetail>(response);

            return domain;
        }

        return default!;
    }

    public async Task<PlaylistDetail[]> GetPlaylistAsync(long uid, int limit = 30, int offset = 0)
    {
        var result = await Searcher.GetAsync(
            UserSelector.GetPlaylist()
                .WithParam("uid", uid.ToString())
                .WithParam("limit", limit.ToString())
                .WithParam("offset", offset.ToString())
                .Build());

        var response = result.ToEntity<UserPlaylistModel>();

        if (response is not null)
        {
            foreach (var item in response.Playlist)
            {
                item.Cover += "?param=512y512";
            }

            var domain = Mapper.Map<PlaylistDetail[]>(response);

            return domain;
        }

        return default!;
    }
}