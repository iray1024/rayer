using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Login.QrCode;

public class QrCodeKeyModel : ResponseBase
{
    public QrCodeKeyDetail Data { get; set; } = default!;

    public class QrCodeKeyDetail : ResponseBase
    {
        public string Unikey { get; set; } = string.Empty;
    }
}