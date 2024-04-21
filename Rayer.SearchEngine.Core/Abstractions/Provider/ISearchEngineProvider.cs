namespace Rayer.SearchEngine.Core.Abstractions.Provider;

public interface ISearchEngineProvider
{
    ISearchEngine SearchEngine { get; }
}