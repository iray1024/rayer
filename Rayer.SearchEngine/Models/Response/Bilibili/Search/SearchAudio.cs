namespace Rayer.SearchEngine.Models.Response.Bilibili.Search;

public class SearchAudio : ResponseBase
{
    public string Message { get; set; } = string.Empty;

    public int TTL { get; set; }

    public SearchAudioDetail Data { get; set; } = null!;
}