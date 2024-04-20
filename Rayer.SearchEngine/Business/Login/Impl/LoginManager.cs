using Microsoft.Extensions.DependencyInjection;
using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Http.Abstractions;
using Rayer.SearchEngine.Business.Login.Abstractions;
using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Models.Response.Netease.Login.User;

namespace Rayer.SearchEngine.Business.Login.Impl;

[Inject<ILoginManager>]
internal class LoginManager : SearchEngineBase, ILoginManager
{
    private readonly IServiceProvider _serviceProvider;

    public LoginManager(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;

        AccountInfo = new AccountInfo();
    }

    public AccountInfo AccountInfo { get; private set; }

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
        _ = await Searcher.GetAsync(Login.RefreshLogin().Build());

        AppCore.GetRequiredService<ICookieManager>().StoreCookie();
    }

    public IAnonymousService UseAnonymous()
    {
        return _serviceProvider.GetRequiredService<IAnonymousService>();
    }

    public IPhoneService UsePhone()
    {
        return _serviceProvider.GetRequiredService<IPhoneService>();
    }

    public IQrCodeService UseQrCode()
    {
        return _serviceProvider.GetRequiredService<IQrCodeService>();
    }

    public void RaiseLoginSucceed()
    {
        LoginSucceed?.Invoke(this, EventArgs.Empty);
    }

    public async Task<AccountInfo> GetAccountInfoAsync(CancellationToken cancellationToken = default)
    {
        var result = await Searcher.GetAsync(Account.AccountInfo().Build());

        var response = result.ToEntity<AccountInfo>();

        if (response is not null)
        {
            AccountInfo = response;

            return response;
        }

        return default!;
    }
}