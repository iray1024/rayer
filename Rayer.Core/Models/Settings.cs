using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Windows.Input;
using Wpf.Ui.Appearance;
using static Rayer.Core.Constants;

namespace Rayer.Core.Models;

[Serializable]
[JsonConverter(typeof(SettingsJsonConverter))]
public class Settings : ISettings
{
    public ObservableCollection<string> AudioLibrary { get; init; } = [];

    public ApplicationTheme Theme { get; set; } = ApplicationTheme.Light;

    public PlaySingleAudioStrategy PlaySingleAudioStrategy { get; set; } = PlaySingleAudioStrategy.AddToQueue;
    public PlayloopMode PlayloopMode { get; set; } = PlayloopMode.List;
    public ImmersiveMode ImmersiveMode { get; set; } = ImmersiveMode.Vinyl;
    public float Volume { get; set; } = 1.0f;
    public float Pitch { get; set; } = 1.0f;

    public KeyBinding KeyPlayOrPause { get; set; } = DefaultKeyBinding.KeyPlayOrPause;
    public KeyBinding KeyPrevious { get; set; } = DefaultKeyBinding.KeyPrevious;
    public KeyBinding KeyNext { get; set; } = DefaultKeyBinding.KeyNext;
    public KeyBinding KeyPitchUp { get; set; } = DefaultKeyBinding.KeyPitchUp;
    public KeyBinding KeyPitchDown { get; set; } = DefaultKeyBinding.KeyPitchDown;
    public KeyBinding KeyForward { get; set; } = DefaultKeyBinding.KeyForward;
    public KeyBinding KeyRewind { get; set; } = DefaultKeyBinding.KeyRewind;
}