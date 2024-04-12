namespace Rayer.Core.Lyric.Models;

public class KugouTranslation
{
    [JsonPropertyName("content")]
    public List<ContentItem>? Content { get; set; }

    public class ContentItem
    {
        [JsonPropertyName("language")]
        public int Language { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("lyricContent")]
        public List<List<string>?>? LyricContent { get; set; }
    }

    [JsonPropertyName("version")]
    public int Version { get; set; }
}