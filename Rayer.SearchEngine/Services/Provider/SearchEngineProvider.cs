using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Abstractions;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Services.Provider;

[Inject<ISearchEngineProvider>]
internal class SearchEngineProvider : ISearchEngineProvider
{
    private readonly SearchEngineOptions _searchEngineOptions;

    public SearchEngineProvider(SearchEngineOptions searchEngineOptions)
    {
        _searchEngineOptions = searchEngineOptions;
    }

    public ISearchEngine SearchEngine => GetSearchEngine();

    private ISearchEngine GetSearchEngine()
    {
        return AppCore.GetRequiredKeyedService<ISearchEngine>(_searchEngineOptions.SearcherType);
    }
}