using Microsoft.Extensions.DependencyInjection;
using Rayer.Core;
using Rayer.Core.Http;

namespace Rayer.SearchEngine.Bilibili.Extensions;

public static class ServiceProviderExtensions
{
    public static IServiceProvider UseBilibili(this IServiceProvider provider)
    {
        var register = provider.GetRequiredService<ICookieRegister>();

        register.Register("bilibili.com", new Uri("https://www.bilibili.com"), Constants.Paths.BilibiliCookiePath);

        return provider;
    }
}