using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class UserApiSelector(SearchEngineOptions options) : ApiSelector
{
    public IParamBuilder GetLikelist()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.User.GetLikelist);
    }

    public IParamBuilder GetPlaylist()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.User.GetPlaylist);
    }
}