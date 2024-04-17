using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Rayer.Services;

[Inject<INavigationCustomHeaderController>]
internal class NavigationCustomHeaderController(INavigationService navigationService) : INavigationCustomHeaderController
{
    public void Show(object content)
    {
        if (navigationService.GetNavigationControl() is INavigationView navView)
        {
            navView.HeaderVisibility = Visibility.Visible;

            var mainWindow = App.MainWindow;

            mainWindow.Presenter.Content = content;
            mainWindow.PageHeaderContainer.Visibility = Visibility.Visible;
            mainWindow.PageHeaderContainer.Margin = new Thickness(0, 32, 0, 0);
            mainWindow.PageHeader.Visibility = Visibility.Collapsed;
            mainWindow.Presenter.Visibility = Visibility.Visible;
        }
    }

    public void Hide()
    {
        var mainWindow = App.MainWindow;

        mainWindow.Presenter.Content = default!;

        mainWindow.PageHeaderContainer.Margin = new Thickness(32, 32, 42, 20);
        mainWindow.PageHeader.Visibility = Visibility.Visible;
        mainWindow.Presenter.Visibility = Visibility.Collapsed;
    }
}