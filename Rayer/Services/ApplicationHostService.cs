using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rayer.Core;
using Rayer.FrameworkCore;
using Rayer.Views.Pages;
using System.Windows;

namespace Rayer.Services;

internal class ApplicationHostService(IServiceProvider _serviceProvider) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return HandleActivationAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private Task HandleActivationAsync()
    {
        if (Application.Current.Windows.OfType<MainWindow>().Any())
        {
            return Task.CompletedTask;
        }

        var mainWindow = _serviceProvider.GetRequiredService<IWindow>();

        mainWindow.Loaded += OnMainWindowLoaded;
        mainWindow?.Show();

        _serviceProvider.GetRequiredService<ProcessMessageWindow>().Show();

        return Task.CompletedTask;
    }

    private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not MainWindow mainWindow)
        {
            return;
        }

        _ = mainWindow.NavigationView.Navigate(typeof(AudioLibraryPage));
    }
}