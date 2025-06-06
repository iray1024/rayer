using Microsoft.Extensions.Options;
using Rayer.Core.Common;
using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Abstractions;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Services.Provider;

[Inject<ISearchEngineProvider>]
internal class SearchEngineProvider(IOptionsSnapshot<SearchEngineOptions> snapshot) : ISearchEngineProvider
{
    private readonly SearchEngineOptions _searchEngineOptions = snapshot.Value;

    public ISearchEngine SearchEngine => GetSearchEngine();

    SearcherType ISearchProvider.CurrentSearcher => _searchEngineOptions.SearcherType;

    ISearchEngine ISearchEngineProvider.GetSearchEngine(SearcherType searcherType)
    {
        return AppCore.GetRequiredKeyedService<ISearchEngine>(searcherType);
    }

    private ISearchEngine GetSearchEngine()
    {
        return AppCore.GetRequiredKeyedService<ISearchEngine>(_searchEngineOptions.SearcherType);
    }
}