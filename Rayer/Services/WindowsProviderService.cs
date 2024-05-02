using Microsoft.Extensions.DependencyInjection;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Utils;
using System.Windows;

namespace Rayer.Services;

[Inject<IWindowsProviderService>]
internal class WindowsProviderService(IServiceProvider _serviceProvider) : IWindowsProviderService
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

        if (!ElementHelper.IsWindowOpen<T>())
        {
            instance.Owner = Application.Current.MainWindow;
            instance.Show();
        }
    }
}