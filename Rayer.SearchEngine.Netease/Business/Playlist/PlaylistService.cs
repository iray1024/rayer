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
            var domain = Mapper.Map<PlaylistDetail>(response);

            domain.Type = SearchType.Playlist;
            domain.Cover += "?param=512y512";

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

                if (audio.Album is not null)
                {
                    audio.Album.Cover += "?param=512y512";
                }
            }

            return domain;
        }

        return default!;
    }
}