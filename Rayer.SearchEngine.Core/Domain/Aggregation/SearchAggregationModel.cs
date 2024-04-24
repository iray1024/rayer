using Rayer.Core.Common;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Album;

namespace Rayer.SearchEngine.Core.Domain.Aggregation;

public record SearchAggregationModel(SearcherType searcherType)
{
    public string QueryText { get; set; } = string.Empty;

    public SearchAudio Audio { get; set; } = null!;

    public SearchAlbum Album { get; set; } = null!;

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