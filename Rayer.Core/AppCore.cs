using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows;

namespace Rayer.Core;

public static class AppCore
{
    private static readonly CancellationTokenSource _cancellationTokenSource = new();
    private static IServiceProvider _serviceProvider = default!;

    public static CancellationToken StoppingToken => _cancellationTokenSource.Token;

    public static Window MainWindow => Application.Current.MainWindow;

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