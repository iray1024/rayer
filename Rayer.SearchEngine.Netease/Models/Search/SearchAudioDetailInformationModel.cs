using System.ComponentModel;

namespace Rayer.SearchEngine.Netease.Models.Search;

public class SearchAudioDetailInformationModel
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("ar")]
    public ArtistModel[] Artists { get; set; } = [];

    [JsonPropertyName("al")]
    public AlbumModel? Album { get; set; }

    [Description("热度")]
    public float Pop { get; set; }

    public int Fee { get; set; }

    [JsonPropertyName("dt")]
    public long Duration { get; set; }

    public int OriginCoverType { get; set; }

    [JsonPropertyName("noCopyrightRcmd")]
    public NoCopyrightDetail? NoCopyright { get; set; }

    public record NoCopyrightDetail
    {
        public int Type { get; set; }

        public string TypeDesc { get; set; } = string.Empty;
    }
}