using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rayer.Abstractions;
using Rayer.Command;
using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Playing;
using Rayer.SearchEngine.Extensions;
using Rayer.Services;
using Rayer.ViewModels;
using Rayer.Views.Pages;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Rayer;

public partial class App : Application
{
    private static readonly CancellationTokenSource _cancellationTokenSource = new();

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
            services.AddSingleton<ImmersivePlayerViewModel>();
            services.AddSingleton<PlaybarViewModel>();
            services.AddSingleton<RightPlaybarPanelViewModel>();
            services.AddSingleton<ProcessMessageWindow>();

            services.AddSingleton<AudioLibraryPage>();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();

            services.AddSingleton<IPlaybarService, PlaybarService>();
            services.AddSingleton<IPlaylistService, PlaylistService>();
            services.AddSingleton<IThemeResourceProvider, ThemeResourceProvider>();
            services.AddSingleton<IContextMenuFactory, ContextMenuFactory>();
            services.AddSingleton<ICommandBinding, CommandBindingService>();
            services.AddSingleton<IImmersivePlayerService, ImmersivePlayerService>();
            services.AddSingleton<IImmersivePresenterProvider, ImmersivePresenterProvider>();

            services.AddSingleton<WindowsProviderService>();

            services.AddRayerCore();
            services.AddSearchEngine(builder =>
            {
                builder.SetHttpEndpoint("https://netease-cloud-music-api-rayer.vercel.app");
            });

            services.AddTransientFromNamespace("Rayer.Views", RayerAssembly.Assembly);
            services.AddTransientFromNamespace("Rayer.ViewModels", RayerAssembly.Assembly);
        })
        .Build();

    public static CancellationToken StoppingToken => _cancellationTokenSource.Token;

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

    protected override void OnStartup(StartupEventArgs e)
    {
        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

        _ = GetRequiredService<ISettingsService>();
        var watcher = GetRequiredService<IAudioFileWatcher>();

        watcher.Watch();

        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        AppCore.UseServiceProvider(_host.Services);

        _host.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        var playback = GetService<Playback>();
        var watcher = GetService<IAudioFileWatcher>();

        playback?.Dispose();
        watcher?.Dispose();

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

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
        var dialogService = GetService<IContentDialogService>();

        if (dialogService is not null)
        {
            await Current.Dispatcher.InvokeAsync(async () =>
            {
                await dialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions
                {
                    Title = "异常",
                    Content = $"{ex?.Message}\n{ex?.StackTrace}",
                    CloseButtonText = "关闭"
                }, StoppingToken);
            });
        }
    }
}