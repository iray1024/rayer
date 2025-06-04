namespace Rayer.Updater.Models;

public class Release
{
    [JsonPropertyName("tag_name")]
    public string Tag { get; set; } = null!;

    [JsonIgnore]
    public Version Version { get; set; } = default!;

    public Assets[] Assets { get; set; } = [];
}

public class Assets
{
    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    [JsonPropertyName("browser_download_url")]
    public string DownloadUrl { get; set; } = null!;
}