using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Bilibili.Models.Search;

internal class WebAudioModel : ResponseBase
{
    public string Message { get; set; } = string.Empty;

    public int TTL { get; set; }

    public WebAudioDetailModel Data { get; set; } = null!;

    public record WebAudioDetailModel
    {
        public string Format { get; set; } = string.Empty;

        public WebAudioDashModel Dash { get; set; } = null!;

        public record WebAudioDashModel
        {
            public int Duration { get; set; }

            public WebAudioDashAudioModel[] Audio { get; set; } = [];

            public record WebAudioDashAudioModel
            {
                public long Id { get; set; }

                public string BaseUrl { get; set; } = string.Empty;
            }
        }
    }
}