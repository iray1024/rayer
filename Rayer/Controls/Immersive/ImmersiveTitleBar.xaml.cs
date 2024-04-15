using Rayer.Abstractions;
using Rayer.Core.Abstractions;
using Rayer.Core.Events;
using Rayer.Core.Framework;
using Rayer.Core.PInvoke;
using Rayer.Core.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

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

    private static Rect _lastWndState;
    private void OnMaximumMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var hwnd = new WindowInteropHelper(_mainWindow).Handle;

        var lStyle = Win32.User32.GetWindowLong(hwnd, (int)Win32.WINDOW_LONG_PTR_INDEX.GWL_STYLE);

        if (lStyle != 386334720)
        {
            _lastWndState.Size = new Size(_mainWindow.Width, _mainWindow.Height);
            _lastWndState.Location = new Point(_mainWindow.Left, _mainWindow.Top);

            ElementHelper.FullScreen(_mainWindow);
        }
        else
        {
            ElementHelper.EndFullScreen(_mainWindow);

            _mainWindow.Width = _lastWndState.Size.Width;
            _mainWindow.Height = _lastWndState.Size.Height;
        }
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