using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Abstractions;
using Rayer.Core.Events;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.PlayControl.Abstractions;

namespace Rayer.ViewModels;

[Inject]
public partial class PlaybarViewModel : ObservableObject
{
    private readonly IAudioManager _audioManager;
    private readonly IPlaybarService _playbarService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private Audio? _current;

    [ObservableProperty]
    private string _currentTime = "00:00";

    [ObservableProperty]
    private string _totalTime = "00:00";

    [ObservableProperty]
    private double _progressValue = 0d;

    [ObservableProperty]
    private double _progressWidth = (1200 - 400) / 2.0;

    [ObservableProperty]
    private bool _extraElementVisiable = false;

    public PlaybarViewModel(
        IAudioManager audioManager,
        IPlaybarService playbarService,
        ISettingsService settingsService)
    {
        _audioManager = audioManager;
        _playbarService = playbarService;
        _settingsService = settingsService;

        _audioManager.Playback.DispatcherTimer.Tick += UpdateProgress;
        _audioManager.AudioChanged += OnSwitch;
        _audioManager.AudioStopped += OnStopped;
    }

    public IAudioManager AudioManager => _audioManager;
    public IPlaybarService PlaybarService => _playbarService;
    public ISettingsService SettingsService => _settingsService;

    public bool IgnoreUpdateProgressValue { get; set; } = false;

    private void UpdateProgress(object? sender, EventArgs e)
    {
        CurrentTime = _audioManager.Playback.CurrentTime.Hours == 0
            ? _audioManager.Playback.CurrentTime.ToString(@"mm\:ss")
            : _audioManager.Playback.CurrentTime.ToString(@"hh\:mm\:ss");

        TotalTime = _audioManager.Playback.TotalTime.Hours == 0
            ? _audioManager.Playback.TotalTime.ToString(@"mm\:ss")
            : _audioManager.Playback.TotalTime.ToString(@"hh\:mm\:ss");

        if (!IgnoreUpdateProgressValue)
        {
            ProgressValue = _audioManager.Playback.CurrentTime / _audioManager.Playback.TotalTime * 100.0d;
        }
    }

    public void OnSwitch(object? sender, AudioChangedArgs e)
    {
        Current = e.New;
        Playing?.Invoke(null, EventArgs.Empty);
        OnPropertyChanged(nameof(Current));
    }

    public void OnStopped(object? sender, EventArgs e)
    {
        CurrentTime = "00:00";
        TotalTime = "00:00";
        ProgressValue = 0d;

        Current = null;

        Stopped?.Invoke(null, EventArgs.Empty);
    }

    public event EventHandler? Playing;
    public event EventHandler? Stopped;
}