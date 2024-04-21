namespace Rayer.SearchEngine.Core.Business.Login;

public interface ILoginManager
{
    Domain.Authority.User Account { get; }

    IPhoneService UsePhone();

    IQrCodeService UseQrCode();

    IAnonymousService UseAnonymous();

    Task RefreshLoginStateAsync(CancellationToken cancellationToken = default);

    Task<bool> LogoutAsync(CancellationToken cancellationToken = default);

    Task GetUserDetailAsync(CancellationToken cancellationToken = default);

    void RaiseLoginSucceed();

    Task<Domain.Authority.User> GetAccountInfoAsync(CancellationToken cancellationToken = default);

    event EventHandler LoginSucceed;
}