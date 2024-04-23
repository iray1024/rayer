namespace Rayer.SearchEngine.Core.Domain.Authority;

public record User
{
    public Account Account { get; set; } = new();

    public Profile? Profile { get; set; }
}