namespace Rayer.SearchEngine.Core.Domain.Common;

public record Lyric
{
    public LyricDetailModel Klyric { get; set; } = null!;

    public LyricDetailModel Lrc { get; set; } = null!;

    public LyricDetailModel Romalrc { get; set; } = null!;

    public LyricDetailModel Tlyric { get; set; } = null!;

    public record LyricDetailModel
    {
        public string Lyric { get; set; } = string.Empty;

        public int Version { get; set; }
    }
}