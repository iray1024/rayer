namespace Rayer.SearchEngine.Models.Response.Netease.Login.QrCode;

public class QrCode : ResponseBase
{
    public QrCodeResponseDetail Data { get; set; } = default!;

    public class QrCodeResponseDetail
    {
        [JsonPropertyName("qrurl")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("qrimg")]
        public string Image { get; set; } = string.Empty;
    }
}