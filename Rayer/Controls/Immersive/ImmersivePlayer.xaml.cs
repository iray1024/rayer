using Rayer.Core;
using Rayer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rayer.Controls.Immersive;

public partial class ImmersivePlayer : UserControl
{
    public ImmersivePlayer()
    {
        ViewModel = App.GetRequiredService<ImmersivePlayerViewModel>();
        DataContext = this;

        InitializeComponent();
    }

    public ImmersivePlayerViewModel ViewModel { get; set; }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            App.MainWindow.DragMove();
        }

        if (e.ClickCount == 2)
        {
            var mainwindow = AppCore.MainWindow;

            mainwindow.WindowState = mainwindow.WindowState is WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }
    }
}