namespace Rayer.SearchEngine.Netease.Models.Search;

public class WebAudioModel
{
    public WebAudioDetail[] Data { get; set; } = [];

    public class WebAudioDetail
    {
        public long Id { get; set; }

        public string Url { get; set; } = string.Empty;
    }
}