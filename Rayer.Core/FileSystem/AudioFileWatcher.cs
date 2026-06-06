using Rayer.Core.FileSystem.Abstractions;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Models;
using Rayer.Core.Utils;
using Rayer.FrameworkCore.Injection;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace Rayer.Core.FileSystem;

[Inject<IAudioFileWatcher>]
internal class AudioFileWatcher : IAudioFileWatcher
{
    private readonly ISettingsService _settingsService;

    private static readonly string[] _filters = IAudioFileWatcher.MediaFilter.Split('|');

    private readonly ObservableCollection<FileSystemWatcher> _watchers = [];

    private readonly ConcurrentDictionary<string, byte> _knownPaths = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, byte> _scanningWatcherPaths = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, byte> _pathsCreatedDuringScan = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 这个变量表示整个系统监控的所有Audio
    /// </summary>
    public ObservableCollection<Audio> Audios { get; } = [];

    public AudioFileWatcher(ISettingsService settingsService)
    {
        MediaRecognizer.Initialize();

        _settingsService = settingsService;
        var libs = settingsService.Settings.AudioLibrary;

        _watchers = new ObservableCollection<FileSystemWatcher>(libs.Select(x => new FileSystemWatcher(x)));

        _watchers.CollectionChanged += WatchersChanged;
    }

    public event EventHandler? PreLoaded;

    private void WatchersChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action is NotifyCollectionChangedAction.Add)
        {
            if (e.NewItems is not null)
            {
                foreach (var item in e.NewItems)
                {
                    if (_settingsService.Settings.AsyncFileSystem)
                    {
                        AttatchAsync((FileSystemWatcher)item);
                    }
                    else
                    {
                        Attach((FileSystemWatcher)item);
                    }
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

    private Task AttatchAsync(FileSystemWatcher watcher)
    {
        return Task.Run(() =>
        {
            Attach(watcher);
        });
    }

    private void Attach(FileSystemWatcher watcher)
    {
        watcher.Changed += Watcher_Changed;
        watcher.Created += Watcher_Created;
        watcher.Deleted += Watcher_Deleted;
        watcher.Renamed += Watcher_Renamed;

        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;

        _scanningWatcherPaths[watcher.Path] = 0;

        try
        {
            var files = Directory
                .GetFiles(watcher.Path, "*", SearchOption.AllDirectories)
                .Where(ValidFileType);

            foreach (var file in files)
            {
                TryRecognizeAndAdd(file);
            }

            ProcessPathsCreatedDuringScan(watcher.Path);
        }
        finally
        {
            _scanningWatcherPaths.TryRemove(watcher.Path, out _);
        }
    }

    private void Detatch(FileSystemWatcher watcher)
    {
        Task.Run(() =>
        {
            watcher.EnableRaisingEvents = false;

            watcher.Changed -= Watcher_Changed;
            watcher.Created -= Watcher_Created;
            watcher.Deleted -= Watcher_Deleted;
            watcher.Renamed -= Watcher_Renamed;

            var files = Directory
                .GetFiles(watcher.Path, "*", SearchOption.AllDirectories)
                .Where(ValidFileType);

            foreach (var file in files)
            {
                _knownPaths.TryRemove(file, out _);
                _pathsCreatedDuringScan.TryRemove(file, out _);

                var target = Audios.FirstOrDefault(x => x.Path.Equals(file, StringComparison.OrdinalIgnoreCase));

                if (target is not null)
                {
                    Audios.Remove(target);
                }
            }
        });
    }

    public void Watch()
    {
        if (_settingsService.Settings.AsyncFileSystem)
        {
            var tasks = new List<Task>();
            foreach (var watcher in _watchers)
            {
                tasks.Add(AttatchAsync(watcher));
            }

            _ = Task.WhenAll(tasks).ContinueWith(task => PreLoaded?.Invoke(this, EventArgs.Empty));
        }
        else
        {
            foreach (var watcher in _watchers)
            {
                Attach(watcher);
            }

            PreLoaded?.Invoke(this, EventArgs.Empty);
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
            _knownPaths.TryRemove(e.FullPath, out _);
            _pathsCreatedDuringScan.TryRemove(e.FullPath, out _);

            var target = Audios.FirstOrDefault(x => x.Path.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase));

            if (target is not null)
            {
                Audios.Remove(target);
            }
        }
    }

    private async void Watcher_Created(object sender, FileSystemEventArgs e)
    {
        if (!ValidFileType(e.FullPath))
        {
            return;
        }

        if (sender is FileSystemWatcher watcher && _scanningWatcherPaths.ContainsKey(watcher.Path))
        {
            _pathsCreatedDuringScan.TryAdd(e.FullPath, 0);
            return;
        }

        await WaitFileOperationCompletedAsync(e.FullPath);

        TryRecognizeAndAdd(e.FullPath);
    }

    private void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        if (ValidFileType(e.FullPath) && !_knownPaths.ContainsKey(e.FullPath))
        {
            TryRecognizeAndAdd(e.FullPath);
        }
    }

    private void TryRecognizeAndAdd(string path)
    {
        if (_knownPaths.ContainsKey(path))
        {
            return;
        }

        try
        {
            var audio = MediaRecognizer.Recognize(path);

            if (_knownPaths.TryAdd(audio.Path, 0))
            {
                Audios.Add(audio);
            }
        }
        catch
        {
        }
    }

    private void ProcessPathsCreatedDuringScan(string watcherRoot)
    {
        var pendingPaths = _pathsCreatedDuringScan.Keys
            .Where(path => IsUnderPath(path, watcherRoot) && !_knownPaths.ContainsKey(path))
            .ToList();

        foreach (var path in pendingPaths)
        {
            _pathsCreatedDuringScan.TryRemove(path, out _);
            TryRecognizeAndAdd(path);
        }
    }

    private static bool IsUnderPath(string filePath, string rootPath)
    {
        var normalizedRoot = Path.GetFullPath(rootPath)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        return Path.GetFullPath(filePath).StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase);
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