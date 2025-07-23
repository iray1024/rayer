using Rayer.Core.Common;
using Rayer.Core.Http;
using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Business.Login;
using Rayer.SearchEngine.Netease.Engine;
using Rayer.SearchEngine.Netease.Models.Login.Authority;

namespace Rayer.SearchEngine.Netease.Business.Login;

[Inject<ILoginManager>]
internal class LoginManager : SearchEngineBase, ILoginManager
{
    public LoginManager()
    {
        Account = new Core.Domain.Authority.User();
    }

    public Core.Domain.Authority.User Account { get; private set; }

    public event EventHandler? LoginSucceed;

    public Task GetUserDetailAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LogoutAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task RefreshLoginStateAsync(CancellationToken cancellationToken = default)
    {
        _ = await Searcher.GetAsync(LoginSelector.RefreshLogin().Build());

        AppCore.GetRequiredService<ICookieManager>().StoreCookie();
    }

    public IAnonymousService UseAnonymous()
    {
        return AppCore.GetRequiredService<IAnonymousService>();
    }

    public IPhoneService UsePhone()
    {
        return AppCore.GetRequiredKeyedService<IPhoneService>(SearcherType.Netease);
    }

    public IQrCodeService UseQrCode()
    {
        return AppCore.GetRequiredKeyedService<IQrCodeService>(SearcherType.Netease);
    }

    public void RaiseLoginSucceed()
    {
        LoginSucceed?.Invoke(this, EventArgs.Empty);
    }

    public async Task<Core.Domain.Authority.User> GetAccountInfoAsync(CancellationToken cancellationToken = default)
    {
        var result = await Searcher.GetAsync(AccountSelector.AccountInfo().Build());

        var response = result.ToEntity<UserModel>();
        if (response is not null)
        {
            Account = Mapper.Map<Core.Domain.Authority.User>(response);

            return Account;
        }

        return default!;
    }
}