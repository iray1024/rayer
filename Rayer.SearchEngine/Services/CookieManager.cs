using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Http.Abstractions;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Rayer.SearchEngine.Services;

[Inject<ICookieManager>]
internal partial class CookieManager : ICookieManager
{
    private readonly HttpClientHandler _handler;
    private readonly CookieContainer _cookieContainer;
    private static readonly Uri _csrfUri = new("https://netease-cloud-music-api-rayer.vercel.app");

    public CookieManager()
    {
        _cookieContainer = new CookieContainer();

        _handler = new HttpClientHandler
        {
            Proxy = new WebProxy(new Uri("http://127.0.0.1:10809"), true),
            UseProxy = true,
            CookieContainer = _cookieContainer
        };
    }

    public HttpClientHandler HttpClientHandler => _handler;

    public CookieContainer CookieContainer => _handler.CookieContainer;

    public void StoreCookie()
    {
        var cookies = CookieContainer.GetAllCookies();

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

    public void SetCookies(string cookies)
    {
        var slices = CookieRegex.Matches(cookies);

        var currentHttpEndpoint = AppCore.GetRequiredService<SearchEngineOptions>().HttpEndpoint;

        var domain = currentHttpEndpoint.Split("//")[1].Split(':')[0];

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

            _cookieContainer.Add(new Uri(currentHttpEndpoint), cookie);
        }
    }

    private static void StoreCookie(string cookies)
    {
        using var stream = new FileStream(Constants.Paths.CookiePath, FileMode.Create, FileAccess.Write, FileShare.Read);

        using var writer = new StreamWriter(stream, Encoding.UTF8);

        writer.Write(cookies);

        writer.Flush();
        writer.Close();
    }

    private readonly Regex CookieRegex = CookieParser();

    [GeneratedRegex(@"(?<Name>[^=]+)=(?<Value>[^;]+); Max-Age=(?<MaxAge>[^;]+); Expires=(?<Expires>[^;]+); Path=(?<Path>[^;]+);")]
    private static partial Regex CookieParser();
}