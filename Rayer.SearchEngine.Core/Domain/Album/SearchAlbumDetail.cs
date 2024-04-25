using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Artist;
using Rayer.SearchEngine.Core.Domain.Common;

namespace Rayer.SearchEngine.Core.Domain.Album;

public record SearchAlbumDetail
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public ArtistDefinition[] Artists { get; set; } = [];

    public string Cover { get; set; } = string.Empty;

    public int AudioCount { get; set; }

    public SearchAudioDetail[] Audios { get; set; } = [];

    public Copyright Copyright { get; set; } = Copyright.Default;

    public IDictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
}