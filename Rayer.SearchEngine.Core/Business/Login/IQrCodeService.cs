using Rayer.SearchEngine.Core.Domain.Authority.Login;

namespace Rayer.SearchEngine.Core.Business.Login;

public interface IQrCodeService
{
    Task<QrCode> GetQrCodeAsync(CancellationToken cancellationToken = default);

    Task<QrCodeVerify> CheckAsync(CancellationToken cancellationToken = default);
}