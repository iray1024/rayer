namespace Rayer.SearchEngine.Models.Response.Search;

public class SearchAudioDetail : ResponseBase
{
    [JsonPropertyName("songs")]
    public SearchAudioDetailInformation[] Details { get; set; } = [];

    public Privileges[] Privileges { get; set; } = [];
}