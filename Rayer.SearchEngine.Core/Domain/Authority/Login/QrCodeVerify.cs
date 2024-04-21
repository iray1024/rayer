namespace Rayer.SearchEngine.Core.Domain.Authority.Login;

public class QrCodeVerify
{
    public int Code { get; set; }

    public string Message { get; set; } = string.Empty;

    public string Cookie { get; set; } = string.Empty;
}