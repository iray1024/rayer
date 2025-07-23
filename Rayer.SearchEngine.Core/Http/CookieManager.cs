using Microsoft.Extensions.Options;
using Rayer.Core;
using Rayer.Core.Http;
using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using Rayer.SearchEngine.Core.Options;
using System.Net;
using System.Text.RegularExpressions;

namespace Rayer.SearchEngine.Core.Http;

[Inject<ICookieManager>]
internal partial class CookieManager : ICookieManager
{
    private readonly HttpClientHandler _handler;
    private readonly CookieContainer _cookieContainer;

    public CookieManager()
    {
        _cookieContainer = new CookieContainer();

        _handler = new HttpClientHandler
        {
            Proxy = HttpClient.DefaultProxy,
            UseProxy = false,
            CookieContainer = _cookieContainer
        };
    }

    public HttpClientHandler HttpClientHandler => _handler;

    public CookieContainer CookieContainer => _handler.CookieContainer;

    public void SetCookies(string domain, Uri uri, string cookies)
    {
        var slices = CookieRegex.Matches(cookies);

        if (slices.Count > 0)
        {
            foreach (var match in slices.Cast<Match>())
            {
                var name = match.Groups["Name"].Value.Trim().Replace("/;", "");
                var value = match.Groups["Value"].Value.Trim();
                var expires = DateTime.Parse(match.Groups["Expires"].Value.Trim());
                var path = match.Groups["Path"].Value.Trim();
                var httpOnly = match.Groups["HTTPOnly"].Success;

                var cookie = new Cookie(name, value, path, domain)
                {
                    Expires = expires,
                    HttpOnly = httpOnly,
                };

                _cookieContainer.Add(uri, cookie);
            }
        }
        else
        {
            var sampleSlices = SampleCookieRegex.Matches(cookies);

            if (sampleSlices.Count > 0)
            {
                foreach (var match in sampleSlices.Cast<Match>())
                {
                    var name = match.Groups["Name"].Value.Trim().Replace("/;", "");
                    var value = match.Groups["Value"].Value.Trim();

                    var cookie = new Cookie(name, value, "/", domain);

                    _cookieContainer.Add(uri, cookie);
                }
            }
        }
    }

    public void StoreCookie()
    {
        var currentHttpEndpoint = AppCore.GetRequiredService<IOptionsSnapshot<SearchEngineOptions>>().Value.HttpEndpoint;

        var cookies = CookieContainer.GetCookies(new Uri(currentHttpEndpoint));

        var sb = new StringBuilder();

        foreach (var cookie in cookies.Cast<Cookie>())
        {
            sb.Append(
                $"{cookie.Name}={cookie.Value}; " +
                $"Max-Age={(cookie.Expires - cookie.TimeStamp).TotalSeconds:0}; " +
                $"Expires={cookie.Expires.ToUniversalTime().ToString("R")}; " +
                $"Path={cookie.Path};" +
                $"{(false ? " HTTPOnly;" : string.Empty)}");
        }

        var cookiesStr = sb.ToString();

        StoreCookie(cookiesStr);
    }

    private static void StoreCookie(string cookies)
    {
        using var stream = new FileStream(Constants.Paths.CookiePath, FileMode.Create, FileAccess.Write, FileShare.Read);

        using var writer = new StreamWriter(stream, Encoding.UTF8);

        writer.Write(cookies);

        writer.Flush();
        writer.Close();
    }

    private readonly Regex CookieRegex = ComprehensiveCookieParser();
    private readonly Regex SampleCookieRegex = SampleCookieParser();

    [GeneratedRegex(@"(?<Name>[^=]+)=(?<Value>[^;]+); Max-Age=(?<MaxAge>[^;]+); Expires=(?<Expires>[^;]+); Path=(?<Path>[^;]+);")]
    private static partial Regex ComprehensiveCookieParser();

    [GeneratedRegex(@"(?<Name>[^=]+)=(?<Value>[^;]+); ")]
    private static partial Regex SampleCookieParser();
}