using Microsoft.Extensions.Options;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class UserApiSelector(IOptionsSnapshot<SearchEngineOptions> snapshot) : ApiSelector(snapshot)
{
    public IParamBuilder GetLikelist()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.User.GetLikelist);
    }

    public IParamBuilder GetFavPlaylist()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.User.GetFavPlaylist);
    }

    public IParamBuilder GetFavAlbumlist()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.User.GetFavAlbumlist);
    }
}