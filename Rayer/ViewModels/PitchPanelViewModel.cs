using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Abstractions;
using System.Runtime.CompilerServices;

namespace Rayer.ViewModels;

public partial class PitchPanelViewModel : ObservableObject
{
    private readonly IAudioManager _audioManager;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private float _value;

    public PitchPanelViewModel(
        IAudioManager audioManager,
        ISettingsService settingsService)
    {
        _audioManager = audioManager;
        _settingsService = settingsService;

        Value = TransformFromFactor(_settingsService.Settings.Pitch);
    }

    public ISettingsService SettingsService => _settingsService;

    public void Reset()
    {
        Value = TransformFromFactor(1f);

        _audioManager.Playback.Pitch = 1f;
        _settingsService.Settings.Pitch = 1f;
        _settingsService.Save();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float TransformFromFactor(float factor)
    {
        return (factor - 0.5f) * (100 / 1.5f);
    }
}