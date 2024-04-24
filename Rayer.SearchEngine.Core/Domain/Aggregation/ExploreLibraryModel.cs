using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Album;
using Rayer.SearchEngine.Core.Domain.Playlist;

namespace Rayer.SearchEngine.Core.Domain.Aggregation;

public record ExploreLibraryModel
{
    public int LikeCount { get; set; }

    public string[] RandomLyrics { get; set; } = [];

    public PlaylistDetail FavoriteList { get; set; } = null!;

    public SearchAudioDetail[] PainedLikeAudios { get; set; } = [];

    public ExploreLibraryDetailModel Detail { get; set; } = new();

    public record ExploreLibraryDetailModel
    {
        public PlaylistDetail[] Playlist { get; set; } = [];

        public Album.Album[] FavAlbum { get; set; } = [];

    }
}