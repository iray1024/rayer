using Rayer.Core.Framework.Injection;
using Rayer.Core.Http.Abstractions;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Rayer.SearchEngine.Services;

[Inject<ICookieManager>]
internal partial class CookieManager : ICookieManager
{
    private readonly HttpClientHandler _handler = new();
    private readonly CookieContainer _cookieContainer;
    private static readonly Uri _csrfUri = new("https://netease-cloud-music-api-rayer.vercel.app");

    public CookieManager()
    {
        _cookieContainer = new CookieContainer();

        _handler.CookieContainer = _cookieContainer;
    }

    public HttpClientHandler HttpClientHandler => _handler;

    public CookieContainer CookieContainer => _handler.CookieContainer;

    public void SetCookies(string cookies)
    {
        var slices = CookieRegex.Matches(cookies);

        foreach (var match in slices.Cast<Match>())
        {
            var name = match.Groups["Name"].Value.Trim().Replace("/;", "");
            var value = match.Groups["Value"].Value.Trim();
            var expires = DateTime.Parse(match.Groups["Expires"].Value.Trim());
            var path = match.Groups["Path"].Value.Trim();
            var httpOnly = match.Groups["HTTPOnly"].Success;

            var cookie = new Cookie(name, value, path)
            {
                Expires = expires,
                HttpOnly = httpOnly,
            };

            _cookieContainer.Add(_csrfUri, cookie);
        }
    }

    private readonly Regex CookieRegex = CookieParser();

    [GeneratedRegex(@"(?<Name>[^=]+)=(?<Value>[^;]+); Max-Age=(?<MaxAge>[^;]+); Expires=(?<Expires>[^;]+); Path=(?<Path>[^;]+);(;?\s*\(?\s*(?<HTTPOnly>HTTPOnly)\s*\)?)?;")]
    private static partial Regex CookieParser();
}