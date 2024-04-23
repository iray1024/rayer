namespace Rayer.SearchEngine.Core.Domain.Authority.Login;

public record QrCode
{
    public long Id { get; set; }

    public string Image { get; set; } = string.Empty;
}