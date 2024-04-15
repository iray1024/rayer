namespace Rayer.SearchEngine.Models.Response.Search;

public class SearchSuggestResponse : ResponseBase
{
    public SearchSuggectResponseDetail Result { get; set; } = null!;

    public class SearchSuggectResponseDetail
    {
        public SearchSuggectResponseAlbumDetail[] Albums { get; set; } = [];

        [JsonPropertyName("songs")]
        public SearchSuggectResponseAudioDetail[] Audios { get; set; } = [];
    }

    public class SearchSuggectResponseAlbumDetail
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public class SearchSuggectResponseAudioDetail
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}