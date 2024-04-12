using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Abstractions;

namespace Rayer.ViewModels;

public abstract class ImmersiveViewModelBase(IAudioManager audioManager) : ObservableObject
{
    private static readonly Audio _fallbackAudio = new();

    protected readonly IAudioManager _audioManager = audioManager;

    protected internal virtual Audio Clone(Audio? source)
    {
        return source is not null
            ? new Audio
            {
                Artists = source.Artists,
                Title = source.Title,
                Album = source.Album,
                Duration = source.Duration,
                Cover = source.Cover,
                Path = source.Path
            }
            : _fallbackAudio;
    }
}