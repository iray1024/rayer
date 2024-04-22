using Microsoft.Extensions.Options;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

internal class ApiSelector(IOptionsSnapshot<SearchEngineOptions> snapshot) : ApiSelectorBase
{
    protected readonly SearchEngineOptions _searchEngineOptions = snapshot.Value;

    protected override IParamBuilder CreateBuilder(string httpEndpoint, string apiEndpoint)
    {
        return new ParamBuilder(httpEndpoint, apiEndpoint);
    }
}