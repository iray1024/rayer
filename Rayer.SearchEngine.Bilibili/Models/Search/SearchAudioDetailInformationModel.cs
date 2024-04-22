namespace Rayer.SearchEngine.Bilibili.Models.Search;

public class SearchAudioDetailInformationModel
{
    public long Id { get; set; }

    public string Type { get; set; } = "video";

    public string Author { get; set; } = string.Empty;

    public string TypeId { get; set; } = string.Empty;

    public string TypeName { get; set; } = string.Empty;

    public long Aid { get; set; }

    public string Bvid { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Pic { get; set; } = string.Empty;

    public int Play { get; set; }

    [JsonPropertyName("video_review")]
    public int VideoReview { get; set; }

    public int Favorites { get; set; }

    public int Like { get; set; }

    public string Tag { get; set; } = string.Empty;

    public string Duration { get; set; } = string.Empty;

    [JsonPropertyName("rank_score")]
    public long RankScore { get; set; }

    [JsonPropertyName("upic")]
    public string AuthorPicture { get; set; } = string.Empty;
}