using Rayer.Core.Framework.Injection;
using Rayer.Core.Http;
using System.Collections.ObjectModel;

namespace Rayer.SearchEngine.Core.Http;

[Inject<ICookieRegister>]
internal class CookieRegister : ICookieRegister
{
    private readonly IDictionary<(string Domain, Uri Uri), string?> _cookieRegisterCenter = new Dictionary<(string Domain, Uri Uri), string?>();

    public bool Register(string domain, Uri uri, string? cachePath = null)
    {
        return _cookieRegisterCenter.TryAdd((domain, uri), cachePath);
    }

    public ReadOnlyDictionary<(string Domain, Uri Uri), string?> GetCookieRegisterCenter()
    {
        return _cookieRegisterCenter.AsReadOnly();
    }
}