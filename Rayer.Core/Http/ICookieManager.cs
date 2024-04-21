using System.Net;
using System.Net.Http;

namespace Rayer.Core.Http;

public interface ICookieManager
{
    HttpClientHandler HttpClientHandler { get; }

    CookieContainer CookieContainer { get; }

    void SetCookies(string domain, Uri uri, string cookies);

    void StoreCookie();
}