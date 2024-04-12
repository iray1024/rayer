using Rayer.SearchEngine.Internal.Abstractions;

namespace Rayer.SearchEngine.Internal.ApiSelector;

internal class AccountApiSelector(SearchEngineOptions options) : ApiSelectorBase(options)
{
    public ParamBuilder UserDetail()
    {
        return CreateBuilder(ApiEndpoints.Account.UserDetail);
    }

    public ParamBuilder UserInfo()
    {
        return CreateBuilder(ApiEndpoints.Account.UserInfo);
    }

    public ParamBuilder AccountInfo()
    {
        return CreateBuilder(ApiEndpoints.Account.AccountInfo);
    }
}