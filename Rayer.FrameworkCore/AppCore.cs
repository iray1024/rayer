using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows;

namespace Rayer.FrameworkCore;

public static class AppCore
{
    private static readonly CancellationTokenSource _cancellationTokenSource = new();
    private static IServiceProvider _serviceProvider = default!;

    public static IServiceProvider ServiceProvider => _serviceProvider;

    public static CancellationToken StoppingToken => _cancellationTokenSource.Token;

    public static Window MainWindow => Application.Current is not null
        ? Application.Current.MainWindow
        : null!;

    public static T GetRequiredService<T>()
        where T : class
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    public static T GetRequiredKeyedService<T>(object? serviceKey)
         where T : notnull
    {
        return _serviceProvider.GetRequiredKeyedService<T>(serviceKey);
    }

    public static T? GetService<T>()
        where T : class
    {
        return _serviceProvider.GetService<T>();
    }

    public static object? GetService(Type type)
    {
        return _serviceProvider.GetService(type);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void UseServiceProvider(in IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public static void Exit()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}