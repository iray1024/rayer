namespace Rayer.SearchEngine.Core.Domain.Common;

public class Lyric
{
    public LyricDetailModel Klyric { get; set; } = null!;

    public LyricDetailModel Lrc { get; set; } = null!;

    public LyricDetailModel Romalrc { get; set; } = null!;

    public LyricDetailModel Tlyric { get; set; } = null!;

    public class LyricDetailModel
    {
        public string Lyric { get; set; } = string.Empty;

        public int Version { get; set; }
    }
}