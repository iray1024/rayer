using Rayer.SearchEngine.Models.Response.Search;

namespace Rayer.SearchEngine.Abstractions;

public interface ISearchAware
{
    public Task OnSearchAsync(SearchAggregationModel model);
}