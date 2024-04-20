namespace Rayer.SearchEngine.Models.Response.Netease.User;

public class UserLikelist : ResponseBase
{
    public long[] Ids { get; set; } = [];

    public long CheckPoint { get; set; }
}