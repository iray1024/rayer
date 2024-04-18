namespace Rayer.SearchEngine.Models.Response.User;

public class UserLikelistResponse : ResponseBase
{
    public long[] Ids { get; set; } = [];

    public long CheckPoint { get; set; }
}