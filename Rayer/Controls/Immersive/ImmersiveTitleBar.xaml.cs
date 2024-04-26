using Rayer.Abstractions;
using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Events;
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

    private void OnMaximumMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_mainWindow.WindowState is WindowState.Normal)
        {
            _mainWindow.ResizeMode = ResizeMode.NoResize;
            _mainWindow.WindowState = WindowState.Maximized;
        }
        else
        {
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.ResizeMode = ResizeMode.CanResize;
        }
    }

    private void OnCloseMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _mainWindow.Close();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _mainWindow = AppCore.GetRequiredService<MainWindow>();
    }
}