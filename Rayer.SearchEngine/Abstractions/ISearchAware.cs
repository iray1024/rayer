using Rayer.SearchEngine.Models.Response.Netease.Search;

namespace Rayer.SearchEngine.Abstractions;

public interface ISearchAware
{
    public void OnSearch(SearchAggregationModel model);
}