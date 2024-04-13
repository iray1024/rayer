namespace Rayer.SearchEngine.Models.Response.Search;

public class SearchAudioResposne : ResponseBase
{
    public SearchAudioResult Result { get; set; } = null!;
}

public class SearchAudioResult
{
    public SearchAudioResultDetail[] Songs { get; set; } = [];

    [JsonPropertyName("hasMore")]
    public bool HasMore { get; set; }

    [JsonPropertyName("songCount")]
    public int Count { get; set; }
}

public class SearchAudioResultDetail
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public SearchAudioArtistsDetail[] Artists { get; set; } = [];

    public SearchAudioAlbumDetail Album { get; set; } = null!;

    public long Duration { get; set; }

    public int Status { get; set; }

    public int Fee { get; set; }
}

public class SearchAudioArtistsDetail
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

public class SearchAudioAlbumDetail
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Status { get; set; }
}