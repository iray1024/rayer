using Rayer.SearchEngine.Core.Domain.Aggregation;

namespace Rayer.SearchEngine.Abstractions;

public interface ISearchAware
{
    public void OnSearch(SearchAggregationModel model);
}