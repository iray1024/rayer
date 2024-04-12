namespace Rayer.Core.Lyric.Models;

public class CreditsInfo
{
    [JsonPropertyName("t")]
    public int Timestamp { get; set; }

    [JsonPropertyName("c")]
    public List<Credit> Credits { get; set; } = [];

    public class Credit
    {
        [JsonPropertyName("tx")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("li")]
        public string Image { get; set; } = string.Empty;

        [JsonPropertyName("or")]
        public string Orpheus { get; set; } = string.Empty;
    }
}