namespace Rayer.SearchEngine.Core.Domain.Authority;

public record Account
{
    public long Id { get; set; }

    public int Status { get; set; }

    public int VipType { get; set; }

    public bool Anonimous { get; set; } = true;
}