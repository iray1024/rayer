using Rayer.Abstractions;
using Rayer.Core.Abstractions;
using Rayer.Core.Events;
using Rayer.Core.Framework;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rayer.Controls.Immersive;

public partial class ImmersiveTitleBar : UserControl
{
    private MainWindow _mainWindow = default!;
    private readonly IAudioManager _audioManager;

    public ImmersiveTitleBar()
    {
        _audioManager = App.GetRequiredService<IAudioManager>();

        InitializeComponent();

        _audioManager.AudioPlaying += OnPlaying;
        _audioManager.AudioPaused += OnPaused;
        _audioManager.AudioStopped += OnStopped;
    }

    private void OnPlaying(object? sender, AudioPlayingArgs e)
    {
        Switch.IsEnabled = true;
    }

    private void OnPaused(object? sender, EventArgs e)
    {
        Switch.IsEnabled = false;
    }

    private void OnStopped(object? sender, EventArgs e)
    {
        Switch.IsEnabled = false;
    }

    private async void OnSwitchMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        await App.GetRequiredService<IImmersivePlayerService>().Switch();
    }

    private void OnMinimumMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _mainWindow.WindowState = WindowState.Minimized;
    }

    private static Rect _lastWindowPostion;
    private static readonly WindowState _lastWindowState;
    private static WindowState _currentWindowState;
    private static Size _beforeMaximizedSize;

    private readonly bool _isNowProgramControl = false;
    private void OnMaximumMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _mainWindow.WindowState = _mainWindow.WindowState is WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        /*
        var hwnd = new WindowInteropHelper(_mainWindow).Handle;

        var lStyle = Win32.User32.GetWindowLong(hwnd, (int)Win32.WINDOW_LONG_PTR_INDEX.GWL_STYLE);

        if (lStyle != 386334720)
        {
            _lastWindowPostion.Size = new Size(_mainWindow.ActualWidth, _mainWindow.ActualHeight);
            _lastWindowPostion.Location = new Point(_mainWindow.Left, _mainWindow.Top);

            ElementHelper.FullScreen(_mainWindow);
        }
        else
        {
            _isNowProgramControl = true;
            ElementHelper.EndFullScreen(_mainWindow);
            _isNowProgramControl = false;

            if (_currentWindowState is WindowState.Maximized)
            {
                _mainWindow.Opacity = 0;
                _mainWindow.Width = _beforeMaximizedSize.Width;
                _mainWindow.Height = _beforeMaximizedSize.Height;

                var currentScreen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(_mainWindow).Handle);

                _mainWindow.Left = ((currentScreen.Bounds.Width - _mainWindow.ActualWidth) / 2) + currentScreen.Bounds.Left;
                _mainWindow.Top = ((currentScreen.Bounds.Height - _mainWindow.ActualHeight) / 2) + currentScreen.Bounds.Top;

                //_mainWindow.WindowState = WindowState.Normal;
            }
        }*/
    }

    private void OnCloseMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _mainWindow.Close();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _mainWindow = (MainWindow)App.GetRequiredService<IWindow>();

        _mainWindow.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(object? sender, EventArgs e)
    {
        if (!_isNowProgramControl)
        {
            _currentWindowState = _mainWindow.WindowState;

            if (_currentWindowState is WindowState.Maximized)
            {
                _beforeMaximizedSize = new Size(_mainWindow.ActualWidth, _mainWindow.ActualHeight);
            }
        }
    }
}