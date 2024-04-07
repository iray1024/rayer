using Rayer.Core.Models;
using System.Collections.ObjectModel;

namespace Rayer.Core.Abstractions;

public interface IAudioFileWatcher : IDisposable
{
    const string MediaFilter = ".flac|.fla|.mka|.mp4|.m4a|.mp3|.ogg|.opus|.aac|.wav|.wma";

    ObservableCollection<Audio> Audios { get; }

    void Watch();

    void AddWatcher(string path);
    void RemoveWatcher(string path);
}