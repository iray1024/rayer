using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Business.Playlist.Abstractions;
using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Models.Response.User;

namespace Rayer.SearchEngine.Business.Playlist.Impl;

[Inject<IPlaylistService>]
internal class PlaylistService : SearchEngineBase, IPlaylistService
{
    public PlaylistService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public async Task<PlaylistDetailResponse> GetPlaylistDetailAsync(long id)
    {
        var result = await Searcher.GetAsync(
            Playlist.GetPlaylistDetail()
                .WithParam("id", id.ToString())
                .Build());

        var response = result.ToEntity<PlaylistDetailResponse>();

        return response is not null ? response : default!;
    }
}