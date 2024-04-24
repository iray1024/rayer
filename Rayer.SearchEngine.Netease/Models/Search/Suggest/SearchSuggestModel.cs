using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Netease.Models.Search.Album;
using Rayer.SearchEngine.Netease.Models.Search.Audio;

namespace Rayer.SearchEngine.Netease.Models.Search.Suggest;

public class SearchSuggestModel : ResponseBase
{
    public SearchSuggectDetailModel Result { get; set; } = null!;

    public record SearchSuggectDetailModel
    {
        public SearchAlbumSuggestInformationModel[] Albums { get; set; } = [];

        [JsonPropertyName("songs")]
        public SearchAudioDetailInformationModel[] Audios { get; set; } = [];
    }
}