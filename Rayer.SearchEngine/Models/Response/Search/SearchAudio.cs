namespace Rayer.SearchEngine.Models.Response.Search;

public class SearchAudio : ResponseBase
{
    public SearchAudioResult Result { get; set; } = null!;

    public class SearchAudioResult
    {
        public SearchAudioDetailInformation[] Songs { get; set; } = [];

        [JsonPropertyName("hasMore")]
        public bool HasMore { get; set; }

        [JsonPropertyName("songCount")]
        public int Count { get; set; }
    }
}