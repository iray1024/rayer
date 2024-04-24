using Rayer.SearchEngine.Core.Domain.Artist;
using Rayer.SearchEngine.Core.Domain.Common;

namespace Rayer.SearchEngine.Core.Domain.Album;

public class SearchAlbumDetail
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public ArtistDefinition[] Artists { get; set; } = [];

    public string Cover { get; set; } = string.Empty;

    public Copyright Copyright { get; set; } = Copyright.Default;

    public IDictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
}