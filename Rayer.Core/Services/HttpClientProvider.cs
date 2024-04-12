using Rayer.Core.Abstractions;
using System.Net.Http;

namespace Rayer.Core.Services;

internal class HttpClientProvider : IHttpClientProvider
{
    public HttpClientProvider()
    {
        var cookieManager = AppCore.GetService<ICookieManager>();

        HttpClient = cookieManager is not null
            ? new HttpClient(cookieManager.HttpClientHandler, false)
            : new();
    }

    public HttpClient HttpClient { get; }
}