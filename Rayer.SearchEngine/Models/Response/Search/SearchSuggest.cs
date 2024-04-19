namespace Rayer.SearchEngine.Models.Response.Search;

public class SearchSuggest : ResponseBase
{
    public SearchSuggectDetail Result { get; set; } = null!;

    public class SearchSuggectDetail
    {
        public SearchAlbumDetailInformation[] Albums { get; set; } = [];

        [JsonPropertyName("songs")]
        public SearchAudioDetailInformation[] Audios { get; set; } = [];
    }
}