using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Bilibili.Http.Selector;

[Inject]
internal class SearchApiSelector : ApiSelector
{
    private const string _bilibiliHttpEndpoint = "https://api.bilibili.com";

    public IParamBuilder SearchBvId()
    {
        return CreateBuilder(_bilibiliHttpEndpoint, ApiEndpoints.Search.SearchBvId);
    }

    public IParamBuilder SearchCId()
    {
        return CreateBuilder(_bilibiliHttpEndpoint, ApiEndpoints.Search.SearchCId);
    }

    public IParamBuilder SearchUrl()
    {
        return CreateBuilder(_bilibiliHttpEndpoint, ApiEndpoints.Search.SearchUrl);
    }
}