using Rayer.SearchEngine.Models.Response.Search;
using static Rayer.SearchEngine.Models.Response.User.UserPlaylistResponse;

namespace Rayer.SearchEngine.Models.Domian;

public class ExploreLibraryModel
{
    public int LikeCount { get; set; }

    public string[] RandomLyrics { get; set; } = [];

    public SearchAudioDetailAudioDetail[] PainedLikeAudios { get; set; } = [];

    public SearchAudioDetailAudioDetail[] TotalLikeAudios { get; set; } = [];

    public ExploreLibraryDetailModel Detail { get; set; } = new();

    public class ExploreLibraryDetailModel
    {
        public UserPlaylistReponseDetail[] Playlist { get; set; } = [];

    }
}