using System.ComponentModel;

namespace Rayer.SearchEngine.Models.Response.Search;

public class SearchAudioDetailResponse : ResponseBase
{
    [JsonPropertyName("songs")]
    public SearchAudioDetailAudioDetail[] Details { get; set; } = [];

    public PrivilegesDetail[] Privileges { get; set; } = [];
}

public class SearchAudioDetailAudioDetail
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("ar")]
    public SearchAudioDetailArtistDetail[] Artists { get; set; } = [];

    [JsonPropertyName("al")]
    public SearchAudioDetailAlbumDetail? Album { get; set; }

    [Description("热度")]
    public float Pop { get; set; }

    public int Fee { get; set; }

    [JsonPropertyName("dt")]
    public long Duration { get; set; }

    public int OriginCoverType { get; set; }

    [JsonPropertyName("noCopyrightRcmd")]
    public SearchAudioNoCopyrightDetail? NoCopyright { get; set; }
}

public class SearchAudioDetailArtistDetail
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

public class SearchAudioDetailAlbumDetail
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("picUrl")]
    public string Picture { get; set; } = string.Empty;
}

public class SearchAudioNoCopyrightDetail
{
    public int Type { get; set; }

    public string TypeDesc { get; set; } = string.Empty;
}