using Rayer.Core.Common;

namespace Rayer.SearchEngine.Core.Abstractions.Provider;

public interface ISearchEngineProvider
{
    ISearchEngine SearchEngine { get; }

    ISearchEngine GetSearchEngine(SearcherType searcherType);
}