using Microsoft.Extensions.Options;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class AlbumApiSelector(IOptionsSnapshot<SearchEngineOptions> snapshot) : ApiSelector(snapshot)
{
    public IParamBuilder GetAlbum()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Album.GetAlbum);
    }

    public IParamBuilder GetArtistAlbum()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Album.GetArtistAlbum);
    }
}