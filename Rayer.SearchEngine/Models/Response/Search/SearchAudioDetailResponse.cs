using System.ComponentModel;

namespace Rayer.SearchEngine.Models.Response.Search;

public class SearchAudioDetailResponse : ResponseBase
{
    [JsonPropertyName("songs")]
    public SearchAudioDetailAudioDetail[] Details { get; set; } = [];
}

public class SearchAudioDetailAudioDetail
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("al")]
    public SearchAudioDetailAlbumDetail? Album { get; set; }

    [Description("热度")]
    public float Pop { get; set; }

    public int Fee { get; set; }

    [JsonPropertyName("dt")]
    public long Duration { get; set; }

    public int OriginCoverType { get; set; }
}

public class SearchAudioDetailAlbumDetail
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("picUrl")]
    public string Picture { get; set; } = string.Empty;
}