using Rayer.SearchEngine.Models.Response.Search;

namespace Rayer.SearchEngine.Abstractions;

public interface ISearchEngine
{
    Task<SearchAggregationModel> SearchAsync(string queryText, CancellationToken cancellationToken = default);

    Task<SearchSuggest> SuggestAsync(string keywords, CancellationToken cancellationToken = default);
}