using Rayer.Core.Abstractions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rayer.Controls.Immersive;

public partial class ImmersiveTitleBar : UserControl
{
    private MainWindow _mainWindow = default!;

    public ImmersiveTitleBar()
    {
        InitializeComponent();
    }

    private void OnMinimumMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _mainWindow.WindowState = WindowState.Minimized;
    }

    private void OnMaximumMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _mainWindow.WindowState = _mainWindow.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void OnCloseMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _mainWindow.Close();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _mainWindow = (MainWindow)App.GetRequiredService<IWindow>();
    }
}