using System.Net;
using System.Net.Http;

namespace Rayer.Core.Abstractions;

public interface ICookieManager
{
    HttpClientHandler HttpClientHandler { get; }

    CookieContainer CookieContainer { get; }

    void SetCookies(string cookies);
}