using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Search;

public class SearchSuggestModel : ResponseBase
{
    public SearchSuggectDetailModel Result { get; set; } = null!;

    public record SearchSuggectDetailModel
    {
        public SearchAlbumDetailInformationModel[] Albums { get; set; } = [];

        [JsonPropertyName("songs")]
        public SearchAudioDetailInformationModel[] Audios { get; set; } = [];
    }
}