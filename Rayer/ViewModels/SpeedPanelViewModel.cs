using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Abstractions;
using Rayer.Core.Abstractions;
using Rayer.Core.Framework.Settings.Abstractions;
using System.Windows;
using System.Windows.Controls;

namespace Rayer.ViewModels;

public partial class SpeedPanelViewModel : ObservableObject
{
    private readonly IAudioManager _audioManager;
    private readonly ISettingsService _settingsService;
    private readonly IImmersivePlayerService _immersivePlayerService;

    private DependencyObject _dependency = default!;

    [ObservableProperty]
    private bool _isFlyoutOpen = false;

    [ObservableProperty]
    private float _speed;

    public SpeedPanelViewModel(
        IAudioManager audioManager,
        ISettingsService settingsService,
        IImmersivePlayerService immersivePlayerService)
    {
        _audioManager = audioManager;
        _settingsService = settingsService;
        _immersivePlayerService = immersivePlayerService;

        Speed = _audioManager.Playback.DeviceManager.Speed * 100.0f / 2;
    }

    public void SetDependency(DependencyObject dependency)
    {
        _dependency = dependency;
    }

    public void OnButtonClick()
    {
        if (!IsFlyoutOpen)
        {
            IsFlyoutOpen = true;
        }
    }

    public void OnButtonRightClick()
    {
        Speed = 50f;

        _audioManager.Playback.DeviceManager.Speed = 1f;
        Save();

        ToolTipService.SetToolTip(_dependency, $"速度：100%");
    }

    public void SetSpeed()
    {
        _audioManager.Playback.DeviceManager.Speed = 2 * Speed / 100.0f;

        ToolTipService.SetToolTip(_dependency, $"速度：{(int)(_audioManager.Playback.DeviceManager.Speed * 100)}%");
    }

    public void SetSpeed(float value)
    {
        Speed = value;

        SetSpeed();
    }

    public void Save()
    {
        _settingsService.Settings.Speed = MathF.Round(_audioManager.Playback.DeviceManager.Speed, 2);
        _settingsService.Save();
    }
}