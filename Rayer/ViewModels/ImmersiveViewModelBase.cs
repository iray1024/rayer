using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core.Abstractions;
using Rayer.Core.Models;

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
                Id = source.Id,
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