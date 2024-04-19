namespace Rayer.SearchEngine.Models.Response.Login;

public class AnonymousUser : ResponseBase
{
    public long UserId { get; set; }

    public long CreateTime { get; set; }

    public string Cookie { get; set; } = string.Empty;
}