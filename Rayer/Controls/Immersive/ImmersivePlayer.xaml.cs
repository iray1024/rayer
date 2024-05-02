using Rayer.Core;
using Rayer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Rayer.Controls.Immersive;

public partial class ImmersivePlayer : UserControl
{
    private bool _isMouseOver = false;
    private DateTime _lastMouseMoveTime;

    public ImmersivePlayer()
    {
        ViewModel = App.GetRequiredService<ImmersivePlayerViewModel>();
        DataContext = this;

        InitializeComponent();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        MouseMove += OnMouseMove;
        MouseLeave += OnMouseLeave;
        CompositionTarget.Rendering += OnCompositionTargetRendering;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        MouseMove -= OnMouseMove;
        MouseLeave -= OnMouseLeave;
        CompositionTarget.Rendering -= OnCompositionTargetRendering;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        _lastMouseMoveTime = DateTime.Now;
        _isMouseOver = true;

        Cursor = Cursors.Arrow;
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        _isMouseOver = false;
    }

    private void OnCompositionTargetRendering(object? sender, EventArgs e)
    {
        if (_isMouseOver &&
            (DateTime.Now - _lastMouseMoveTime).TotalSeconds >= 2)
        {
            Cursor = Cursors.None;
        }
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

            if (mainwindow.WindowState is WindowState.Normal)
            {
                mainwindow.ResizeMode = ResizeMode.NoResize;
                mainwindow.WindowState = WindowState.Maximized;
            }
            else
            {
                mainwindow.WindowState = WindowState.Normal;
                mainwindow.ResizeMode = ResizeMode.CanResize;
            }
        }
    }
}