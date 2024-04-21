using Rayer.Core.Http;
using Rayer.SearchEngine.Core.Http.Serialization;
using System.Net.Http.Json;

namespace Rayer.SearchEngine.Core.Http.Abstractions;

public abstract class RequestBase(IHttpClientProvider httpClientProvider)
{
    private readonly HttpClient _httpClient = httpClientProvider.HttpClient;

    public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
    public const string Cookie = "os=pc;osver=Microsoft-Windows-10-Professional-build-16299.125-64bit;appver=2.0.3.131777;channel=netease;__remember_me=true";

    protected HttpClient HttpClient => _httpClient;

    protected virtual string? HttpRefer { get; }

    protected virtual Dictionary<string, string> GetAdditionalHeaders()
    {
        return [];
    }

    protected async Task<string> GetAsync(string url)
    {
        SetRequestHeaders();

        var response = await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();

        return result;
    }

    protected async Task<string> PostAsync(string url, string param)
    {
        SetRequestHeaders();

        var jsonContent = new StringContent(param, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, jsonContent);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();

        return result;
    }

    protected async Task<string> PostAsFormAsync(string url, Dictionary<string, string> @params)
    {
        SetRequestHeaders();

        var formContent = new FormUrlEncodedContent(@params);
        var response = await _httpClient.PostAsync(url, formContent);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();

        return result;
    }

    protected async Task<string> PostAsJsonAsync<T>(string url, T param)
    {
        SetRequestHeaders();

        var response = await _httpClient.PostAsJsonAsync(url, param, JsonExtensions._jsonSerializerOptions);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();

        return result;
    }

    protected virtual void SetRequestHeaders()
    {
        _httpClient.DefaultRequestHeaders.Clear();

        if (!string.IsNullOrEmpty(UserAgent))
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
        }

        if (!string.IsNullOrEmpty(HttpRefer))
        {
            _httpClient.DefaultRequestHeaders.Add("Referer", HttpRefer);
        }

        if (!string.IsNullOrEmpty(Cookie))
        {
            _httpClient.DefaultRequestHeaders.Add("Cookie", Cookie);
        }

        foreach (var pair in GetAdditionalHeaders())
        {
            _httpClient.DefaultRequestHeaders.Add(pair.Key, pair.Value);
        }
    }
}