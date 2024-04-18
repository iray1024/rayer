using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Internal.Abstractions;

namespace Rayer.SearchEngine.Internal.ApiSelector;

[Inject]
internal class PlaylistApiSelector(SearchEngineOptions options) : ApiSelectorBase(options)
{
    public ParamBuilder GetPlaylistDetail()
    {
        return CreateBuilder(ApiEndpoints.Playlist.GetPlaylistDetail);
    }
}