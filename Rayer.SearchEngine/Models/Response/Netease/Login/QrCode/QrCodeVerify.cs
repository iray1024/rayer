namespace Rayer.SearchEngine.Models.Response.Netease.Login.QrCode;

public class QrCodeVerify : ResponseBase
{
    public string Message { get; set; } = string.Empty;

    public string Cookie { get; set; } = string.Empty;
}