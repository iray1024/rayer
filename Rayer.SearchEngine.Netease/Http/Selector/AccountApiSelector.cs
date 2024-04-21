using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class AccountApiSelector(SearchEngineOptions options) : ApiSelector
{
    public IParamBuilder UserDetail()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Account.UserDetail);
    }

    public IParamBuilder UserInfo()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Account.UserInfo);
    }

    public IParamBuilder AccountInfo()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Account.AccountInfo);
    }
}