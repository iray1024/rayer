using Rayer.SearchEngine.Core.Domain.Aggregation;
using Rayer.SearchEngine.Core.Domain.Search;
using Rayer.SearchEngine.Core.Enums;
using System.ComponentModel;

namespace Rayer.SearchEngine.Core.Abstractions;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface ISearchEngine
{
    Task<SearchAggregationModel> SearchAsync(string queryText, SearchType searchType, CancellationToken cancellationToken = default);

    Task<SearchSuggest> SuggestAsync(string keywords, CancellationToken cancellationToken = default);
}