using Rayer.Core.Common;
using Rayer.Core.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Wpf.Ui.Appearance;

namespace Rayer.Core.Framework.Settings.Abstractions;

[JsonConverter(typeof(ISettingsJsonConverter))]
public interface ISettings
{
    public ObservableCollection<string> AudioLibrary { get; }

    public ApplicationTheme Theme { get; set; }

    public PlaySingleAudioStrategy PlaySingleAudioStrategy { get; set; }

    public PlayloopMode PlayloopMode { get; set; }

    public ImmersiveMode ImmersiveMode { get; set; }

    public EqualizerMode EqualizerMode { get; set; }

    public PitchProvider PitchProvider { get; set; }

    public LyricSearcher LyricSearcher { get; set; }

    public SearcherType DefaultSearcher { get; set; }

    public PlaybackRecord PlaybackRecord { get; set; }

    public float Volume { get; set; }

    public float Pitch { get; set; }

    public KeyBinding KeyPlayOrPause { get; set; }
    public KeyBinding KeyPrevious { get; set; }
    public KeyBinding KeyNext { get; set; }
    public KeyBinding KeyPitchUp { get; set; }
    public KeyBinding KeyPitchDown { get; set; }
    public KeyBinding KeyForward { get; set; }
    public KeyBinding KeyRewind { get; set; }
}