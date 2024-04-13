using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Http.Abstractions;
using Rayer.SearchEngine.Business.Login.Abstractions;

namespace Rayer.SearchEngine.ViewModels;

public class ExploreViewModel : ObservableObject
{
    private readonly ILoginManager _loginManager;
    private readonly ICookieManager _cookieManager;

    public ExploreViewModel(ILoginManager loginManager, ICookieManager cookieManager)
    {
        _loginManager = loginManager;
        _cookieManager = cookieManager;
    }

    public async Task LoadAsAnonymousAsync()
    {
        var login = _loginManager.UseAnonymous();

        var response = await login.AnonymousAsync();

        if (response is not null)
        {
            _cookieManager.SetCookies(response.Cookie);
        }
    }
}