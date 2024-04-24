using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Search.Audio;

public class SearchAudioModel : ResponseBase
{
    public SearchAudioResultModel Result { get; set; } = null!;

    public record SearchAudioResultModel
    {
        public SearchAudioDetailInformationModel[] Songs { get; set; } = [];

        [JsonPropertyName("hasMore")]
        public bool HasMore { get; set; }

        [JsonPropertyName("songCount")]
        public int Count { get; set; }
    }
}