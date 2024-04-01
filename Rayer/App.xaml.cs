using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rayer.Core.Abstractions;
using Rayer.Core.Playing;
using Rayer.Services;
using Rayer.ViewModels;
using Rayer.Views.Pages;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;

namespace Rayer;

public partial class App : Application
{
    private static readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(c =>
        {
            c.SetBasePath(AppContext.BaseDirectory);
        })
        .ConfigureServices((_, services) =>
        {
            services.AddHostedService<ApplicationHostService>();

            services.AddSingleton<IWindow, MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<PlaybarViewModel>();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();

            services.AddSingleton<IThemeResourceProvider, ThemeResourceProvider>();

            services.AddSingleton<WindowsProviderService>();

            services.AddRayerCore();

            services.AddSingleton<AudioLibraryPage>();

            services.AddTransientFromNamespace("Rayer.Views", RayerAssembly.Assembly);
            services.AddTransientFromNamespace("Rayer.ViewModels", RayerAssembly.Assembly);
        })
        .Build();

    public static ISnackbarService Snackbar { get; } = new Lazy<ISnackbarService>(GetRequiredService<ISnackbarService>).Value;

    public static T GetRequiredService<T>()
        where T : class
    {
        return _host.Services.GetRequiredService<T>();
    }

    public static T? GetService<T>()
        where T : class
    {
        return _host.Services.GetService<T>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

        _ = GetRequiredService<ISettingsService>();
        var watcher = GetRequiredService<IAudioFileWatcher>();

        watcher.Watch();

        _host.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        var playback = GetService<Playback>();
        var watcher = GetService<IAudioFileWatcher>();

        playback?.Dispose();
        watcher?.Dispose();

        _host.StopAsync().Wait();

        _host.Dispose();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {

    }
}