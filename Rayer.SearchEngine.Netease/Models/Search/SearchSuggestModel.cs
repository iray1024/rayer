using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Search;

public class SearchSuggestModel : ResponseBase
{
    public SearchSuggectDetail Result { get; set; } = null!;

    public class SearchSuggectDetail
    {
        public SearchAlbumDetailInformationModel[] Albums { get; set; } = [];

        [JsonPropertyName("songs")]
        public SearchAudioDetailInformationModel[] Audios { get; set; } = [];
    }
}