namespace Rayer.SearchEngine.Models.Response.Netease.Lyric;

public class LyricResponse : ResponseBase
{
    public LyricResponseDetail Klyric { get; set; } = null!;

    public LyricResponseDetail Lrc { get; set; } = null!;

    public LyricResponseDetail Romalrc { get; set; } = null!;

    public LyricResponseDetail Tlyric { get; set; } = null!;

    public class LyricResponseDetail
    {
        public string Lyric { get; set; } = string.Empty;

        public int Version { get; set; }
    }
}