using Rayer.SearchEngine.Models.Response.Netease.Login.User;

namespace Rayer.SearchEngine.Business.Login.Abstractions;

public interface ILoginManager
{
    AccountInfo AccountInfo { get; }

    IPhoneService UsePhone();

    IQrCodeService UseQrCode();

    IAnonymousService UseAnonymous();

    Task RefreshLoginStateAsync(CancellationToken cancellationToken = default);

    Task<bool> LogoutAsync(CancellationToken cancellationToken = default);

    Task GetUserDetailAsync(CancellationToken cancellationToken = default);

    void RaiseLoginSucceed();

    Task<AccountInfo> GetAccountInfoAsync(CancellationToken cancellationToken = default);

    event EventHandler LoginSucceed;
}