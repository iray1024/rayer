using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Rayer.Core;

public static class AppCore
{
    private static IServiceProvider _serviceProvider = default!;

    public static T GetRequiredService<T>()
        where T : class
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    public static T? GetService<T>()
        where T : class
    {
        return _serviceProvider.GetService<T>();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void UseServiceProvider(in IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
}