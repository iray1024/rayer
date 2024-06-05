using Rayer.Core.Framework.Injection;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui;

namespace Rayer.Core.Framework.Impl;

[Inject<ISnackbarFactory>]
internal class SnackbarFactory(ISnackbarService snackbarService) : ISnackbarFactory
{
    private static readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(2);
    private static readonly ImageSource _logo = (ImageSource)Application.Current.Resources["Logo"];
    private readonly Wpf.Ui.Controls.ImageIcon _logoIcon = new()
    {
        Width = 32,
        Height = 32,
        Source = _logo,
    };

    public void ShowSecondary(string title, string message, TimeSpan? timeout = null)
    {
        timeout ??= _defaultTimeout;

        Application.Current.Dispatcher.Invoke(() =>
        {
            snackbarService.Show(
                title,
                message,
                Wpf.Ui.Controls.ControlAppearance.Secondary,
                _logoIcon,
                timeout.Value);
        });
    }
}