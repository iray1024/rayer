namespace Rayer.SearchEngine.Lyric.Decrypter.Krc;

public class KugouLyricsResponse
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("info")]
    public string? Info { get; set; }

    [JsonPropertyName("_source")]
    public string? Source { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("contenttype")]
    public int ContentType { get; set; }

    [JsonPropertyName("error_code")]
    public int ErrorCode { get; set; }

    [JsonPropertyName("fmt")]
    public string? Format { get; set; }
}