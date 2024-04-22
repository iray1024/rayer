using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Bilibili.Models.Search;

public class SearchAudioModel : ResponseBase
{
    public string Message { get; set; } = string.Empty;

    public int TTL { get; set; }

    public SearchAudioDetailModel Data { get; set; } = null!;
}