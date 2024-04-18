using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Internal.Abstractions;

namespace Rayer.SearchEngine.Internal.ApiSelector;

[Inject]
internal class UserApiSelector(SearchEngineOptions options) : ApiSelectorBase(options)
{
    public ParamBuilder GetLikelist()
    {
        return CreateBuilder(ApiEndpoints.User.GetLikelist);
    }

    public ParamBuilder GetPlaylist()
    {
        return CreateBuilder(ApiEndpoints.User.GetPlaylist);
    }
}