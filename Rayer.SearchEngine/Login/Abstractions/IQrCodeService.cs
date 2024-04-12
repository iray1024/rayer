using Rayer.SearchEngine.Models.Response.QrCode;

namespace Rayer.SearchEngine.Login.Abstractions;

public interface IQrCodeService
{
    Task<QrCodeKeyResponse> GetQrCodeKeyAsync(CancellationToken cancellationToken = default);

    Task<QrCodeResponse> GetQrCodeAsync(string key, CancellationToken cancellationToken = default);

    Task<QrCodeVerifyResponse> CheckAsync(string key, CancellationToken cancellationToken = default);
}