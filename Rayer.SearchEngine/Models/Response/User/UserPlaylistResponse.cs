using Rayer.SearchEngine.Models.Response.Login.User;
using Rayer.SearchEngine.Models.Response.Search;

namespace Rayer.SearchEngine.Models.Response.User;

public class UserPlaylistResponse : ResponseBase
{
    public UserPlaylistReponseDetail[] Playlist { get; set; } = [];

    public bool HasMore { get; set; }

    public string Version { get; set; } = string.Empty;

    public class UserPlaylistReponseDetail
    {
        public long UserId { get; set; }

        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ProfileDetail Creator { get; set; } = null!;

        [JsonPropertyName("coverImgUrl")]
        public string Cover { get; set; } = string.Empty;

        public int TrackCount { get; set; }

        public SearchAudioDetailAudioDetail[] Tracks { get; set; } = [];
    }
}