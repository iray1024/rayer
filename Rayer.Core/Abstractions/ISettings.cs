using Rayer.Core.Common;
using System.Collections.ObjectModel;
using Wpf.Ui.Appearance;

namespace Rayer.Core.Abstractions;

public interface ISettings
{
    public ObservableCollection<string> AudioLibrary { get; }

    public ApplicationTheme Theme { get; set; }

    public PlaySingleAudioStrategy PlaySingleAudioStrategy { get; set; }

    public PlayLoopMode PlayLoopMode { get; set; }

    public float Volume { get; set; }

    public float Pitch { get; set; }
}