using Rayer.Core.Common;
using Rayer.SearchEngine.Core.Domain.Aduio;

namespace Rayer.SearchEngine.Core.Domain.Aggregation;

public class SearchAggregationModel(SearcherType searcherType)
{
    public string QueryText { get; set; } = string.Empty;

    public SearchAudio Audio { get; set; } = null!;

    public SearcherType SearcherType { get; set; } = searcherType;

    public override int GetHashCode()
    {
        return HashCode.Combine(
            QueryText,
            Audio.Page,
            Audio.PageSize,
            Audio.Total,
            SearcherType);
    }
}