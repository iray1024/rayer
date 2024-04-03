using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Abstractions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace Rayer.ViewModels;

public partial class VolumePanelViewModel : ObservableObject
{
    private readonly IAudioManager _audioManager;
    private readonly ISettingsService _settingsService;

    private DependencyObject _dependency = default!;

    [ObservableProperty]
    private bool _isFlyoutOpen = false;

    [ObservableProperty]
    private float _volume;

    public VolumePanelViewModel(
        IAudioManager audioManager,
        ISettingsService settingsService)
    {
        _audioManager = audioManager;
        _settingsService = settingsService;

        Volume = _settingsService.Settings.Volume * 100;
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

    public void SetVolume()
    {
        if (_dependency is ImageIcon image)
        {
            image.Source = Volume == 0f
                ? (ImageSource)Application.Current.Resources["Mute"]
                : Volume > 50f
                ? (ImageSource)Application.Current.Resources["VolumeFull"]
                : (ImageSource)Application.Current.Resources["VolumeHalf"];
        }

        _audioManager.Playback.Volume = Volume / 100.0f;

        ToolTipService.SetToolTip(_dependency, $"音量：{(int)(_settingsService.Settings.Volume * 100)}%");
    }

    public void SetVolume(float value)
    {
        Volume = value;

        SetVolume();
    }

    public void Save()
    {
        _settingsService.Settings.Volume = MathF.Round(Volume / 100.0f, 2);

        _settingsService.Save();
    }
}