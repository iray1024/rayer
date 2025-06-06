using Microsoft.Extensions.Options;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class PlaylistApiSelector(IOptionsSnapshot<SearchEngineOptions> snapshot) : ApiSelector(snapshot)
{
    public IParamBuilder GetPlaylistDetail()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Playlist.GetPlaylistDetail);
    }
}