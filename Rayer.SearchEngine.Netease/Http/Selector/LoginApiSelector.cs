using Microsoft.Extensions.Options;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class LoginApiSelector(IOptionsSnapshot<SearchEngineOptions> snapshot) : ApiSelector(snapshot)
{
    public IParamBuilder Captcha()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Login.Captcha);
    }

    public IParamBuilder CaptchaVerify()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Login.CaptchaVerify);
    }

    public IParamBuilder PhoneLogin()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Login.PhoneLogin);
    }

    public IParamBuilder QrCodeKey()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Login.QrCodeKey);
    }

    public IParamBuilder QrCodeCreate()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Login.QrCodeCreate);
    }

    public IParamBuilder QrCodeVerify()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Login.QrCodeVerify);
    }

    public IParamBuilder AnonymousLogin()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Login.AnonymousLogin);
    }

    public IParamBuilder RefreshLogin()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Login.RefreshLogin);
    }

    public IParamBuilder LoginStatus()
    {
        return CreateBuilder(_searchEngineOptions.HttpEndpoint, ApiEndpoints.Login.LoginStatus);
    }
}