using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Login.QrCode;

public class QrCodeModel : ResponseBase
{
    public QrCodeDetailModel Data { get; set; } = default!;

    public record QrCodeDetailModel
    {
        [JsonPropertyName("qrurl")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("qrimg")]
        public string Image { get; set; } = string.Empty;
    }
}