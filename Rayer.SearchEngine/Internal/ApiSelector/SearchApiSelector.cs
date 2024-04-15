using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Internal.Abstractions;

namespace Rayer.SearchEngine.Internal.ApiSelector;

[Inject]
internal class SearchApiSelector(SearchEngineOptions options) : ApiSelectorBase(options)
{
    public ParamBuilder SearchAudio()
    {
        return CreateBuilder(ApiEndpoints.Search.SampleSearch);
    }

    public ParamBuilder SearchSuggestion()
    {
        return CreateBuilder(ApiEndpoints.Search.SearchSuggestion);
    }
}