using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rayer.Core;
using Rayer.Core.Extensions;
using Rayer.Core.Framework;
using Rayer.Updater.Services;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Rayer.Updater;

public partial class App : Application
{
    private static readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(c =>
        {
            c.SetBasePath(AppContext.BaseDirectory);
        })
        .ConfigureServices((_, services) =>
        {
            services.AddRayerCore();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();
        })
        .Build();

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

        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        AppCore.UseServiceProvider(_host.Services);

        var updater = GetRequiredService<IUpdateService>();
        updater.Initialize(e.Args);

        if (WebRequest.DefaultWebProxy is not null)
        {
            HttpClient.DefaultProxy = WebRequest.DefaultWebProxy;
        }

        var mainWindow = GetRequiredService<IWindow>();
        mainWindow.Show();

        _host.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
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
                });
            });
        }
    }
}