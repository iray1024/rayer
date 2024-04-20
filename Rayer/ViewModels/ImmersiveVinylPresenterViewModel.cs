using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Abstractions;
using Rayer.Core.Events;
using System.Windows;

namespace Rayer.ViewModels;

public partial class ImmersiveVinylPresenterViewModel : ImmersiveViewModelBase
{
    [ObservableProperty]
    private Audio? _current;

    [ObservableProperty]
    private double _currentVinyWidth = 300;

    [ObservableProperty]
    private double _currentCoverWidth = 300;

    [ObservableProperty]
    private double _currentRotateCoverWidth = 300;

    [ObservableProperty]
    private double _currentRotateCoverCanvasTop = 0;

    [ObservableProperty]
    private Thickness _currentPanelMargin = new(0);

    public ImmersiveVinylPresenterViewModel(IAudioManager audioManager)
        : base(audioManager)
    {
        _audioManager.AudioChanged += OnSwitch;
        _audioManager.AudioStopped += OnStopped;

        Current = Clone(_audioManager.Playback.Audio);
    }

    public void OnSwitch(object? sender, AudioChangedArgs e)
    {
        Current = Clone(e.New);

        OnPropertyChanged(nameof(Current));
    }

    public void OnStopped(object? sender, EventArgs e)
    {
        if (Current is not null)
        {
            Current.Cover = null;
            OnPropertyChanged(nameof(Current));
        }
    }
}