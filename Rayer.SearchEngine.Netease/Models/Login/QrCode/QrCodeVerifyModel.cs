using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Login.QrCode;

public class QrCodeVerifyModel : ResponseBase
{
    public string Message { get; set; } = string.Empty;

    public string Cookie { get; set; } = string.Empty;

    public string? NickName { get; set; }

    public string? AvatarUrl { get; set; }
}