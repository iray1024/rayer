namespace Rayer.SearchEngine.Models.Response.Netease.Search;

public class WebAudio : ResponseBase
{
    public WebAudioDetail[] Data { get; set; } = [];

    public class WebAudioDetail
    {
        public long Id { get; set; }

        public string Url { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public int Fee { get; set; }
    }
}