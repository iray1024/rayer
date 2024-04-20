namespace Rayer.SearchEngine.Models.Response.Netease;

public class Album
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("picUrl")]
    public string Picture { get; set; } = string.Empty;
}