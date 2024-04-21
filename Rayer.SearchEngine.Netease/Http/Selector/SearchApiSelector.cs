using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class SearchApiSelector(SearchEngineOptions options) : ApiSelector
{
    public IParamBuilder SearchAudio()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Search.SampleSearch);
    }

    public IParamBuilder SearchSuggestion()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Search.SearchSuggestion);
    }
}