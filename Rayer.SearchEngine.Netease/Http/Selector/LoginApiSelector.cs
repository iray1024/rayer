using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Netease.Http.Selector;

[Inject]
internal class LoginApiSelector(SearchEngineOptions options) : ApiSelector
{
    public IParamBuilder Captcha()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Login.Captcha);
    }

    public IParamBuilder CaptchaVerify()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Login.CaptchaVerify);
    }

    public IParamBuilder PhoneLogin()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Login.PhoneLogin);
    }

    public IParamBuilder QrCodeKey()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Login.QrCodeKey);
    }

    public IParamBuilder QrCodeCreate()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Login.QrCodeCreate);
    }

    public IParamBuilder QrCodeVerify()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Login.QrCodeVerify);
    }

    public IParamBuilder AnonymousLogin()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Login.AnonymousLogin);
    }

    public IParamBuilder RefreshLogin()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Login.RefreshLogin);
    }

    public IParamBuilder LoginStatus()
    {
        return CreateBuilder(options.HttpEndpoint, ApiEndpoints.Login.LoginStatus);
    }
}