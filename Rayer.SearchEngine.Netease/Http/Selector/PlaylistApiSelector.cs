using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class PlaylistApiSelector(SearchEngineOptions options) : ApiSelector
{
    public IParamBuilder GetPlaylistDetail()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Playlist.GetPlaylistDetail);
    }
}