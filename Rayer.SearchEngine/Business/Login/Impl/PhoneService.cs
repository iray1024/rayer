using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Business.Login.Abstractions;
using Rayer.SearchEngine.Internal.Abstractions;

namespace Rayer.SearchEngine.Business.Login.Impl;

[Inject<IPhoneService>(ServiceLifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped)]
internal class PhoneService : SearchEngineBase, IPhoneService
{

    public PhoneService(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {

    }

    public Task LoginAsync(string phone, string password, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task LoginWithCaptchaAsync(string phone, string captcha, CancellationToken cancellationToken = default)
    {
        //var result = await Searcher.GetAsync(UrlBuilder.Build(ApiEndpoints.Login.PhoneLogin, new Dictionary<string, string>()
        //{
        //    ["phone"] = phone,
        //    ["captcha"] = captcha
        //}));
    }

    public async Task SendCaptchaAsync(string phone, CancellationToken cancellationToken = default)
    {
        //var result = await Searcher.GetAsync(UrlBuilder.Build(ApiEndpoints.Login.Captcha, new Dictionary<string, string>() { ["phone"] = phone }));
    }

    public async Task<bool> VerifyCaptchaAsync(string phone, string captcha, CancellationToken cancellationToken = default)
    {
        //var result = await Searcher.GetAsync(UrlBuilder.Build(ApiEndpoints.Login.CaptchaVerify, new Dictionary<string, string>() 
        //{ 
        //    ["phone"] = phone ,
        //    ["captcha"] = captcha
        //}));

        return true;
    }
}