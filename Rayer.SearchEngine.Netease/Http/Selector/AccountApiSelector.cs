using Microsoft.Extensions.Options;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class AccountApiSelector(IOptionsSnapshot<SearchEngineOptions> snapshot) : ApiSelector(snapshot)
{
    public IParamBuilder UserDetail()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Account.UserDetail);
    }

    public IParamBuilder UserInfo()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Account.UserInfo);
    }

    public IParamBuilder AccountInfo()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Account.AccountInfo);
    }
}