namespace Rayer.SearchEngine.Core.Domain.Authority;

public class AnonymousUser
{
    public long UserId { get; set; }

    public string Cookie { get; set; } = string.Empty;
}