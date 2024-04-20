namespace Rayer.SearchEngine.Models.Response.Netease.Login.QrCode;

public class QrCodeKey : ResponseBase
{
    public QrCodeKeyDetail Data { get; set; } = default!;

    public class QrCodeKeyDetail : ResponseBase
    {
        public string Unikey { get; set; } = string.Empty;
    }
}