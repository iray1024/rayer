using Rayer.SearchEngine.Internal.Abstractions;

namespace Rayer.SearchEngine.Internal.ApiSelector;

internal class LoginApiSelector(SearchEngineOptions options) : ApiSelectorBase(options)
{
    public ParamBuilder Captcha()
    {
        return CreateBuilder(ApiEndpoints.Login.Captcha);
    }

    public ParamBuilder CaptchaVerify()
    {
        return CreateBuilder(ApiEndpoints.Login.CaptchaVerify);
    }

    public ParamBuilder PhoneLogin()
    {
        return CreateBuilder(ApiEndpoints.Login.PhoneLogin);
    }

    public ParamBuilder QrCodeKey()
    {
        return CreateBuilder(ApiEndpoints.Login.QrCodeKey);
    }

    public ParamBuilder QrCodeCreate()
    {
        return CreateBuilder(ApiEndpoints.Login.QrCodeCreate);
    }

    public ParamBuilder QrCodeVerify()
    {
        return CreateBuilder(ApiEndpoints.Login.QrCodeVerify);
    }

    public ParamBuilder AnonymousLogin()
    {
        return CreateBuilder(ApiEndpoints.Login.AnonymousLogin);
    }

    public ParamBuilder RefreshLogin()
    {
        return CreateBuilder(ApiEndpoints.Login.RefreshLogin);
    }

    public ParamBuilder LoginStatus()
    {
        return CreateBuilder(ApiEndpoints.Login.LoginStatus);
    }
}