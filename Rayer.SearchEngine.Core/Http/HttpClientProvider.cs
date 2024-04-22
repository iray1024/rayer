using Microsoft.Extensions.Options;
using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Http;
using Rayer.SearchEngine.Core.Options;

namespace Rayer.SearchEngine.Core.Http;

[Inject<IHttpClientProvider>]
internal class HttpClientProvider : IHttpClientProvider
{
    public HttpClientProvider()
    {
        var cookieManager = AppCore.GetRequiredService<ICookieManager>();

        LoadCookie(cookieManager);

        HttpClient = cookieManager is not null
            ? new HttpClient(cookieManager.HttpClientHandler, false)
            : new();
    }

    public HttpClient HttpClient { get; }

    private static void LoadCookie(ICookieManager cookieManager)
    {
        try
        {
            var searchEngineOptions = AppCore.GetRequiredService<IOptionsSnapshot<SearchEngineOptions>>().Value;
            var cookies = File.ReadAllText(Constants.Paths.CookiePath, Encoding.UTF8);

            cookieManager.SetCookies("localhost", new Uri(searchEngineOptions.HttpEndpoint), cookies);
        }
        catch
        { }

        var cookieRegisterCenter = AppCore.GetRequiredService<ICookieRegister>().GetCookieRegisterCenter();

        foreach (var ((domain, uri), cachePath) in cookieRegisterCenter)
        {
            if (!string.IsNullOrEmpty(cachePath))
            {
                try
                {
                    var cookies = File.ReadAllText(cachePath, Encoding.UTF8);

                    cookieManager.SetCookies(domain, uri, cookies);
                }
                catch
                { }
            }
        }
    }
}