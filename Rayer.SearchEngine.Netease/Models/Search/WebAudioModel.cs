namespace Rayer.SearchEngine.Netease.Models.Search;

public record WebAudioModel
{
    public WebAudioDetail[] Data { get; set; } = [];

    public record WebAudioDetail
    {
        public long Id { get; set; }

        public string Url { get; set; } = string.Empty;
    }
}