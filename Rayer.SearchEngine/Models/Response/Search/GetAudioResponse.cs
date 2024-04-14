namespace Rayer.SearchEngine.Models.Response.Search;

public class GetAudioResponse : ResponseBase
{
    public SearchAudioUrlResponseDetail[] Data { get; set; } = [];

    public class SearchAudioUrlResponseDetail
    {
        public long Id { get; set; }

        public string Url { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public int Fee { get; set; }
    }
}
