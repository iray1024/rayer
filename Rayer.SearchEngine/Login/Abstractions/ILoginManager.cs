using Rayer.SearchEngine.Models.Response.User;

namespace Rayer.SearchEngine.Login.Abstractions;

public interface ILoginManager
{
    IPhoneService UsePhone();

    IQrCodeService UseQrCode();

    IAnonymousService UseAnonymous();

    Task RefreshLoginStateAsync(CancellationToken cancellationToken = default);

    Task<bool> LogoutAsync(CancellationToken cancellationToken = default);

    Task GetUserDetailAsync(CancellationToken cancellationToken = default);

    Task<AccountInfoResponse> GetAccountInfoAsync(CancellationToken cancellationToken = default);
}