namespace Rayer.SearchEngine.Models.Response.QrCode;

public class QrCodeVerifyResponse : ResponseBase
{
    public string Message { get; set; } = string.Empty;

    public string Cookie { get; set; } = string.Empty;
}