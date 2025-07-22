namespace Rayer.SearchEngine.Core.Domain.Authority.Login;

public record QrCodeVerify
{
    public int Code { get; set; }

    public string Message { get; set; } = string.Empty;

    public string Cookie { get; set; } = string.Empty;

    public string? NickName { get; set; }

    public string? AvatarUrl { get; set; }
}