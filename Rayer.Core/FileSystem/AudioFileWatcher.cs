using Rayer.Core.Abstractions;
using Rayer.Core.Models;
using Rayer.Core.Utils;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace Rayer.Core.FileSystem;

internal class AudioFileWatcher : IAudioFileWatcher
{
    private static readonly string[] _filters = IAudioFileWatcher.MediaFilter.Split('|');
    private readonly ObservableCollection<FileSystemWatcher> _watchers = [];

    public ObservableCollection<Audio> Audios { get; } = [];

    public AudioFileWatcher(ISettingsService settingsService)
    {
        var libs = settingsService.Settings.AudioLibrary;

        _watchers = new ObservableCollection<FileSystemWatcher>(libs.Select(x => new FileSystemWatcher(x)));

        _watchers.CollectionChanged += WatchersChanged;
    }

    private void WatchersChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action is NotifyCollectionChangedAction.Add)
        {
            if (e.NewItems is not null)
            {
                foreach (var item in e.NewItems)
                {
                    Attatch((FileSystemWatcher)item);
                }
            }
        }
        else if (e.Action is NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems is not null)
            {
                foreach (var item in e.OldItems)
                {
                    Detatch((FileSystemWatcher)item);
                }
            }
        }
    }

    private void Attatch(FileSystemWatcher watcher)
    {
        watcher.Changed += Watcher_Changed;
        watcher.Created += Watcher_Created;
        watcher.Deleted += Watcher_Deleted;
        watcher.Renamed += Watcher_Renamed;

        var files = Directory
            .GetFiles(watcher.Path, "*", SearchOption.AllDirectories)
            .Where(ValidFileType);

        foreach (var item in files.Select(MediaRecognizer.Recognize))
        {
            Audios.Add(item);
        }

        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;
    }

    private void Detatch(FileSystemWatcher watcher)
    {
        watcher.EnableRaisingEvents = false;

        watcher.Changed -= Watcher_Changed;
        watcher.Created -= Watcher_Created;
        watcher.Deleted -= Watcher_Deleted;
        watcher.Renamed -= Watcher_Renamed;

        var files = Directory
            .GetFiles(watcher.Path, "*", SearchOption.AllDirectories)
            .Where(ValidFileType);

        foreach (var item in files.Select(MediaRecognizer.Recognize))
        {
            var target = Audios.FirstOrDefault(x => x.Path == item.Path);

            if (target is not null)
            {
                Audios.Remove(target);
            }
        }
    }

    public void Watch()
    {
        foreach (var watcher in _watchers)
        {
            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Created;
            watcher.Deleted += Watcher_Deleted;
            watcher.Renamed += Watcher_Renamed;

            var files = Directory
                .GetFiles(watcher.Path, "*", SearchOption.AllDirectories)
                .Where(ValidFileType);

            foreach (var item in files.Select(MediaRecognizer.Recognize))
            {
                Audios.Add(item);
            }

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }
    }

    public void AddWatcher(string path)
    {
        var newWatcher = new FileSystemWatcher(path);
        _watchers.Add(newWatcher);
    }

    public void RemoveWatcher(string path)
    {
        var targetWatcher = _watchers.FirstOrDefault(x => x.Path == path);

        if (targetWatcher is not null)
        {
            _watchers.Remove(targetWatcher);
        }
    }

    private void Watcher_Renamed(object sender, RenamedEventArgs e)
    {
        if (ValidFileType(e.FullPath))
        {

        }
    }

    private void Watcher_Deleted(object sender, FileSystemEventArgs e)
    {
        if (ValidFileType(e.FullPath))
        {
            var target = Audios.FirstOrDefault(x => x.Path == e.FullPath);

            if (target is not null)
            {
                Audios.Remove(target);
            }
        }
    }

    private async void Watcher_Created(object sender, FileSystemEventArgs e)
    {
        if (ValidFileType(e.FullPath))
        {
            await WaitFileOperationCompletedAsync(e.FullPath);

            Audios.Add(MediaRecognizer.Recognize(e.FullPath));
        }
    }

    private void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        if (ValidFileType(e.FullPath))
        {

        }
    }

    private static bool ValidFileType(string fileName)
    {
        var extension = Path.GetExtension(fileName);

        return _filters.Contains(extension);
    }

    private static async Task WaitFileOperationCompletedAsync(string path)
    {
    LP:
        try
        {
            using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
        }
        catch (Exception)
        {
            await Task.Delay(100);

            goto LP;
        }
    }

    public void Dispose()
    {
        foreach (var watcher in _watchers)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
    }
}