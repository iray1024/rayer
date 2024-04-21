using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Search;

public class SearchAudioModel : ResponseBase
{
    public SearchAudioResult Result { get; set; } = null!;

    public class SearchAudioResult
    {
        public SearchAudioDetailInformationModel[] Songs { get; set; } = [];

        [JsonPropertyName("hasMore")]
        public bool HasMore { get; set; }

        [JsonPropertyName("songCount")]
        public int Count { get; set; }
    }
}