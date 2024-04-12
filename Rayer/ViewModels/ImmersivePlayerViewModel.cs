using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Abstractions;
using Rayer.Core.Events;

namespace Rayer.ViewModels;

public partial class ImmersivePlayerViewModel : ImmersiveViewModelBase
{
    [ObservableProperty]
    private Audio? _current;

    public ImmersivePlayerViewModel(IAudioManager audioManager)
        : base(audioManager)
    {
        _audioManager.AudioChanged += OnSwitch;
        _audioManager.AudioStopped += OnStopped;

        Current = Clone(audioManager.Playback.Audio);
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