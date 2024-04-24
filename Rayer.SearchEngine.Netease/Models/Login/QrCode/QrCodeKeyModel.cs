using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Login.QrCode;

public class QrCodeKeyModel : ResponseBase
{
    public QrCodeKeyDetailModel Data { get; set; } = default!;

    public class QrCodeKeyDetailModel : ResponseBase
    {
        public string Unikey { get; set; } = string.Empty;
    }
}