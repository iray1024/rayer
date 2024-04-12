namespace Rayer.SearchEngine.Models.Response.QrCode;

public class QrCodeResponse : ResponseBase
{
    public QrCodeResponseDetail Data { get; set; } = default!;
}

public class QrCodeResponseDetail
{
    [JsonPropertyName("qrurl")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("qrimg")]
    public string Image { get; set; } = string.Empty;
}