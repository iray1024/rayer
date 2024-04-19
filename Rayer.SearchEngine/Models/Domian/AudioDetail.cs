using Rayer.SearchEngine.Models.Response;
using Rayer.SearchEngine.Models.Response.Search;
using System.ComponentModel;

namespace Rayer.SearchEngine.Models.Domian;

public class AudioDetail
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Artist[] Artists { get; set; } = [];

    public Album? Album { get; set; }

    [Description("热度")]
    public float Pop { get; set; }

    public int Fee { get; set; }

    [JsonPropertyName("dt")]
    public long Duration { get; set; }

    public int OriginCoverType { get; set; }

    public SearchAudioDetailInformation.NoCopyrightDetail? NoCopyright { get; set; }

    public Privileges Privilege { get; set; } = default!;

    public string NonePlayableReason { get; set; } = string.Empty;
}