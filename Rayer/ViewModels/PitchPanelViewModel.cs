﻿using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Abstractions;
using Rayer.Core.Framework.Settings.Abstractions;

namespace Rayer.ViewModels;

public partial class PitchPanelViewModel : ObservableObject
{
    private readonly IAudioManager _audioManager;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private float _value;

    public float CurrentVaule => MathF.Round(Value, 2);

    public PitchPanelViewModel(
        IAudioManager audioManager,
        ISettingsService settingsService)
    {
        _audioManager = audioManager;
        _settingsService = settingsService;

        Value = _settingsService.Settings.Pitch;
    }

    public ISettingsService SettingsService => _settingsService;

    public void Reset()
    {
        Value = 1f;

        _audioManager.Playback.DeviceManager.Pitch = 1f;
        _settingsService.Settings.Pitch = 1f;
        _settingsService.Save();
    }

    partial void OnValueChanged(float value)
    {
        OnPropertyChanged(nameof(CurrentVaule));
    }
}