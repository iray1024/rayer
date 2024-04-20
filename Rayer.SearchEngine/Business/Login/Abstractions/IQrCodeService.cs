using Rayer.SearchEngine.Models.Response.Netease.Login.QrCode;

namespace Rayer.SearchEngine.Business.Login.Abstractions;

public interface IQrCodeService
{
    Task<QrCodeKey> GetQrCodeKeyAsync(CancellationToken cancellationToken = default);

    Task<QrCode> GetQrCodeAsync(string key, CancellationToken cancellationToken = default);

    Task<QrCodeVerify> CheckAsync(string key, CancellationToken cancellationToken = default);
}