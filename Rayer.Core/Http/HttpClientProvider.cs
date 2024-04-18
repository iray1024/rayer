using Rayer.Core.Framework.Injection;
using Rayer.Core.Http.Abstractions;
using System.Net.Http;

namespace Rayer.Core.Http;

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
            var cookies = System.IO.File.ReadAllText(Constants.Paths.CookiePath, Encoding.UTF8);

            cookieManager.SetCookies(cookies);
        }
        catch
        { }
    }
}