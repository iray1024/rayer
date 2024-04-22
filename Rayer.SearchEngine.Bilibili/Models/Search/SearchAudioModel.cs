using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Models.Response.Bilibili.Search;

public class SearchAudioModel : ResponseBase
{
    public string Message { get; set; } = string.Empty;

    public int TTL { get; set; }

    public SearchAudioDetailModel Data { get; set; } = null!;
}