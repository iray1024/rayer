using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using System.Collections.ObjectModel;
using Wpf.Ui.Appearance;

namespace Rayer.Core.Models;

[Serializable]
internal class Settings : ISettings
{
    public ObservableCollection<string> AudioLibrary { get; init; } = [];

    public ApplicationTheme Theme { get; set; } = ApplicationTheme.Light;

    public PlaySingleAudioStrategy PlaySingleAudioStrategy { get; set; } = PlaySingleAudioStrategy.AddToQueue;
}