using Rayer.Core.Common;
using Rayer.Core.Http;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Http;

[Inject<IRequestService>(ServiceKey = SearcherType.Netease)]
internal class RequestService(IHttpClientProvider httpClientProvider) : RequestBase(httpClientProvider), IRequestService
{
    protected override Dictionary<string, string> GetAdditionalHeaders()
    {
        return base.GetAdditionalHeaders();
    }

    Task<string> IRequestService.GetAsync(string url)
    {
        return GetAsync(url);
    }

    Task<string> IRequestService.PostAsJsonAsync<T>(string url, T param)
    {
        return PostAsJsonAsync(url, param);
    }

    Task<string> IRequestService.PostAsync(string url, string param)
    {
        return PostAsync(url, param);
    }

    Task<string> IRequestService.PostAsFormAsync(string url, Dictionary<string, string> @params)
    {
        return PostAsFormAsync(url, @params);
    }
}