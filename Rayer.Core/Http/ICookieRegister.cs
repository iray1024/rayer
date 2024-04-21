using System.Collections.ObjectModel;

namespace Rayer.Core.Http;

public interface ICookieRegister
{
    bool Register(string domain, Uri uri, string? cachePath = null);

    ReadOnlyDictionary<(string Domain, Uri Uri), string?> GetCookieRegisterCenter();
}