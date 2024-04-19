namespace Rayer.SearchEngine.Models.Response.Login.QrCode;

public class QrCodeKey : ResponseBase
{
    public QrCodeKeyDetail Data { get; set; } = default!;

    public class QrCodeKeyDetail : ResponseBase
    {
        public string Unikey { get; set; } = string.Empty;
    }
}