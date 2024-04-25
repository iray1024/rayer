using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Business.Playlist;
using Rayer.SearchEngine.Core.Domain.Playlist;
using Rayer.SearchEngine.Core.Enums;
using Rayer.SearchEngine.Netease.Engine;
using Rayer.SearchEngine.Netease.Extensions;
using Rayer.SearchEngine.Netease.Models.Search.Playlist;

namespace Rayer.SearchEngine.Netease.Business.Playlist;

[Inject<IPlaylistService>(ServiceKey = SearcherType.Netease)]
internal class PlaylistService : SearchEngineBase, IPlaylistService
{
    public PlaylistService()
    {
    }

    /// <summary>
    /// 根据ID获取歌单详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<PlaylistDetail> GetPlaylistDetailAsync(long id)
    {
        var result = await Searcher.GetAsync(
            PlaylistSelector.GetPlaylistDetail()
                .WithParam("id", id.ToString())
                .Build());

        var response = result.ToEntity<PlaylistDetailModel>();

        if (response is not null)
        {
            response.Playlist.Cover += "?param=512y512";

            foreach (var item in response.Playlist.Tracks)
            {
                if (item.Album is not null && !string.IsNullOrEmpty(item.Album.Cover))
                {
                    item.Album.Cover += "?param=512y512";
                }
            }

            var domain = Mapper.Map<PlaylistDetail>(response);

            domain.Type = SearchType.Playlist;

            for (var i = 0; i < domain.Audios.Length; i++)
            {
                var detail = response.Playlist.Tracks[i];
                var privilege = response.Privileges[i];
                var audio = domain.Audios[i];

                if (!detail.Playable(privilege, out var reason))
                {
                    if (!string.IsNullOrEmpty(reason))
                    {
                        audio.Copyright = new Core.Domain.Common.Copyright { Reason = reason };
                    }
                }
            }

            return domain;
        }

        return default!;
    }
}