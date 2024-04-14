using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using System.Windows;

namespace Rayer.Services;

[Inject<INavigationHeaderUpdater>]
internal class NavigationHeaderUpdater : INavigationHeaderUpdater
{
    public void Show(object content)
    {
        var mainWindow = App.MainWindow;

        var navigationHeaderContentPresenter = mainWindow.Presenter;

        navigationHeaderContentPresenter.Content = content;

        mainWindow.PageHeaderContainer.Margin = new Thickness(0, 32, 0, 0);
        mainWindow.PageHeader.Visibility = Visibility.Collapsed;
        navigationHeaderContentPresenter.Visibility = Visibility.Visible;
    }

    public void Hide()
    {
        var mainWindow = App.MainWindow;

        var navigationHeaderContentPresenter = mainWindow.Presenter;

        navigationHeaderContentPresenter.Content = default!;

        mainWindow.PageHeaderContainer.Margin = new Thickness(32, 32, 42, 20);
        mainWindow.PageHeader.Visibility = Visibility.Visible;
        navigationHeaderContentPresenter.Visibility = Visibility.Collapsed;
    }
}