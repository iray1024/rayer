using Microsoft.Extensions.DependencyInjection;
using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Business.Login;
using Rayer.SearchEngine.Netease.Engine;

namespace Rayer.SearchEngine.Netease.Business.Login;

[Inject<IPhoneService>(ServiceLifetime = ServiceLifetime.Transient, ServiceKey = SearcherType.Netease)]
internal class PhoneService : SearchEngineBase, IPhoneService
{

    public PhoneService()
    {

    }

    public Task LoginAsync(string phone, string password, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task LoginWithCaptchaAsync(string phone, string captcha, CancellationToken cancellationToken = default)
    {
        //var result = await Searcher.GetAsync(UrlBuilder.Build(ApiEndpoints.LoginSelector.PhoneLogin, new Dictionary<string, string>()
        //{
        //    ["phone"] = phone,
        //    ["captcha"] = captcha
        //}));

        return Task.CompletedTask;
    }

    public Task SendCaptchaAsync(string phone, CancellationToken cancellationToken = default)
    {
        //var result = await Searcher.GetAsync(UrlBuilder.Build(ApiEndpoints.LoginSelector.Captcha, new Dictionary<string, string>() { ["phone"] = phone }));

        return Task.CompletedTask;
    }

    public Task<bool> VerifyCaptchaAsync(string phone, string captcha, CancellationToken cancellationToken = default)
    {
        //var result = await Searcher.GetAsync(UrlBuilder.Build(ApiEndpoints.LoginSelector.CaptchaVerify, new Dictionary<string, string>() 
        //{ 
        //    ["phone"] = phone ,
        //    ["captcha"] = captcha
        //}));

        return Task.FromResult(true);
    }
}