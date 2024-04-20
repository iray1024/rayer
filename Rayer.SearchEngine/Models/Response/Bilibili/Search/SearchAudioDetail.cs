namespace Rayer.SearchEngine.Models.Response.Bilibili.Search;

public class SearchAudioDetail
{
    public string Seid { get; set; } = string.Empty;

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int NumResults { get; set; }

    public int NumPages { get; set; }

    [JsonPropertyName("suggest_keyword")]
    public string SuggestKeyword { get; set; } = string.Empty;

    [JsonPropertyName("rqt_type")]
    public string RequestType { get; set; } = "search";

    [JsonPropertyName("egg_hit")]
    public int EggHit { get; set; }

    public SearchAudioDetailInformation[] Result { get; set; } = [];
}