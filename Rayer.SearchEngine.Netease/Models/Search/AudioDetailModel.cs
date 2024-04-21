using System.ComponentModel;

namespace Rayer.SearchEngine.Netease.Models.Search;

public class AudioDetailModel
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ArtistModel[] Artists { get; set; } = [];

    public AlbumModel? Album { get; set; }

    [Description("热度")]
    public float Pop { get; set; }

    public int Fee { get; set; }

    [JsonPropertyName("dt")]
    public long Duration { get; set; }

    public int OriginCoverType { get; set; }

    public SearchAudioDetailInformationModel.NoCopyrightDetail? NoCopyright { get; set; }

    public PrivilegesModel Privilege { get; set; } = default!;

    public string NonePlayableReason { get; set; } = string.Empty;
}