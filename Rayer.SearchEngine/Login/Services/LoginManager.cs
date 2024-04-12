using Microsoft.Extensions.DependencyInjection;
using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Internal.Abstractions;
using Rayer.SearchEngine.Login.Abstractions;
using Rayer.SearchEngine.Models.Response.User;

namespace Rayer.SearchEngine.Login.Services;

internal class LoginManager : SearchEngineBase, ILoginManager
{
    private readonly IServiceProvider _serviceProvider;

    public LoginManager(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task GetUserDetailAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LogoutAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RefreshLoginStateAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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

    public async Task<AccountInfoResponse> GetAccountInfoAsync(CancellationToken cancellationToken = default)
    {
        var result = await Search.GetAsync(Account.AccountInfo().Build());

        var response = result.ToEntity<AccountInfoResponse>();

        return response is not null ? response : default!;
    }
}