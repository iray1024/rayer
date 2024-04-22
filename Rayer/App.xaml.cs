using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rayer.Core;
using Rayer.Core.FileSystem.Abstractions;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Playing;
using Rayer.SearchEngine.Bilibili.Extensions;
using Rayer.SearchEngine.Extensions;
using Rayer.SearchEngine.Netease.Extensions;
using Rayer.Services;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Extensions;

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

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();

            services.UseBilibili();
            services.UseNetease();

            services.AddRayerCore();
            services.AddSearchEngine(builder =>
            {
                builder.SetHttpEndpoint("http://localhost:3000");
            });

            services.AddTransientFromNamespace("Rayer.Views", RayerAssembly.Assembly);
            services.AddTransientFromNamespace("Rayer.ViewModels", RayerAssembly.Assembly);
        })
        .Build();

    public static ISnackbarService Snackbar { get; } = new Lazy<ISnackbarService>(GetRequiredService<ISnackbarService>).Value;

    public static new MainWindow MainWindow => (MainWindow)GetRequiredService<IWindow>();

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

    public static IEnumerable<T> GetServices<T>()
        where T : class
    {
        return _host.Services.GetServices<T>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

        _ = GetRequiredService<ISettingsService>();
        var watcher = GetRequiredService<IAudioFileWatcher>();

        watcher.Watch();

        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        _host.Services.UseBilibili();

        AppCore.UseServiceProvider(_host.Services);

        await Preload();
        CrossThreadAccessor.Initialize();

        _host.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        var playback = GetService<Playback>();
        var watcher = GetService<IAudioFileWatcher>();
        var bootloader = GetService<IIPSBootloader>();

        playback?.Dispose();
        watcher?.Dispose();
        bootloader?.Exit();

        AppCore.Exit();

        _host.StopAsync().Wait();

        _host.Dispose();
    }

    private async void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        e.Handled = true;

        await ShowException(e.Exception);
    }

    private async void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        await ShowException(e.ExceptionObject as Exception);
    }

    private static async Task ShowException(Exception? ex)
    {
        var loader = GetRequiredService<ILoaderProvider>();

        if (loader.IsLoading)
        {
            loader.Loaded();
        }

        if (ex?.Source is "Wpf.Ui")
        {
            return;
        }

        var dialogService = GetService<IContentDialogService>();

        if (dialogService is not null)
        {
            await Current.Dispatcher.InvokeAsync(async () =>
            {
                await dialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions
                {
                    Title = "异常",
                    Content = $"{ex?.Message}",
                    CloseButtonText = "关闭"
                }, AppCore.StoppingToken);
            });
        }
    }

    private static async Task Preload()
    {
        await Current.Dispatcher.InvokeAsync(() =>
        {
            var mapper = GetRequiredService<IMapper>();

            mapper.ConfigurationProvider.CompileMappings();
        });
    }
}