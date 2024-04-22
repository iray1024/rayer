using Rayer.SearchEngine.Core.Domain.Album;
using Rayer.SearchEngine.Core.Domain.Artist;
using Rayer.SearchEngine.Core.Domain.Common;

namespace Rayer.SearchEngine.Core.Domain.Aduio;

public class SearchAudioDetail
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public ArtistDefinition[] Artists { get; set; } = [];

    public AlbumDefinition? Album { get; set; }

    public TimeSpan Duration { get; set; }

    public float Rank { get; set; }

    public Copyright Copyright { get; set; } = Copyright.Default;

    public IDictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
}