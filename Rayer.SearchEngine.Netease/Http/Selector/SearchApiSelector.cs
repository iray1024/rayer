using Microsoft.Extensions.Options;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class SearchApiSelector(IOptionsSnapshot<SearchEngineOptions> snapshot) : ApiSelector(snapshot)
{
    public IParamBuilder SearchAudio()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Search.SampleSearch);
    }

    public IParamBuilder SearchSuggestion()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Search.SearchSuggestion);
    }
}