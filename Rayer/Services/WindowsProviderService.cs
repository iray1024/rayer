using Microsoft.Extensions.DependencyInjection;
using Rayer.Core.Framework.Injection;
using System.Windows;

namespace Rayer.Services;

[Inject]
internal class WindowsProviderService(IServiceProvider _serviceProvider)
{
    public void Show<T>()
        where T : class
    {
        if (!typeof(Window).IsAssignableFrom(typeof(T)))
        {
            throw new InvalidOperationException($"The window class should be derived from {typeof(Window)}.");
        }

        if (_serviceProvider.GetService<T>() is not Window instance)
        {
            throw new InvalidOperationException("Window is not registered as service.");
        }

        instance.Owner = Application.Current.MainWindow;
        instance.Show();
    }
}