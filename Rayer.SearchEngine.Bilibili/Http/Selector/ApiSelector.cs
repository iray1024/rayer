using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Bilibili.Http.Selector;

internal class ApiSelector : ApiSelectorBase
{
    protected override IParamBuilder CreateBuilder(string httpEndpoint, string apiEndpoint)
    {
        return new ParamBuilder(httpEndpoint, apiEndpoint);
    }
}