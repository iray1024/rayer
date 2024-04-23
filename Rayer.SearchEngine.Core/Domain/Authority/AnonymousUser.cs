namespace Rayer.SearchEngine.Core.Domain.Authority;

public record AnonymousUser
{
    public long UserId { get; set; }

    public string Cookie { get; set; } = string.Empty;
}