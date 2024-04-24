using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Bilibili.Models.Search;

internal class SearchCidModel : ResponseBase
{
    public string Message { get; set; } = string.Empty;

    public int TTL { get; set; }

    public SearchCidDetailModel Data { get; set; } = null!;

    public record SearchCidDetailModel
    {
        public string BvId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public long Aid { get; set; }

        public long Cid { get; set; }
    }
}