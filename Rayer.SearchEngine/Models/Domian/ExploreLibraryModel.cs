using Rayer.SearchEngine.Models.Response.User;

namespace Rayer.SearchEngine.Models.Domian;

public class ExploreLibraryModel
{
    public int LikeCount { get; set; }

    public string[] RandomLyrics { get; set; } = [];

    public AudioDetail[] PainedLikeAudios { get; set; } = [];

    public AudioDetail[] TotalLikeAudios { get; set; } = [];

    public ExploreLibraryDetailModel Detail { get; set; } = new();

    public class ExploreLibraryDetailModel
    {
        public PlaylistDetailInformation[] Playlist { get; set; } = [];

    }
}