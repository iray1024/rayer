namespace Rayer.SearchEngine.Models.Response.User;

public class PlaylistDetailResponse : ResponseBase
{
    public UserPlaylistResponse.UserPlaylistReponseDetail Playlist { get; set; } = null!;

    public PrivilegesDetail[] Privileges { get; set; } = [];
}