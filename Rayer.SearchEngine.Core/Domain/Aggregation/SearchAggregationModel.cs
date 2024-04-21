using Rayer.SearchEngine.Core.Domain.Aduio;

namespace Rayer.SearchEngine.Core.Domain.Aggregation;

public class SearchAggregationModel
{
    public string QueryText { get; set; } = string.Empty;

    public SearchAudio Audio { get; set; } = null!;

    public override int GetHashCode()
    {
        return HashCode.Combine(
            QueryText,
            Audio.Page,
            Audio.PageSize);
    }
}