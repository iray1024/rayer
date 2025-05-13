using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Events;
using Rayer.Core.Lyric.Abstractions;
using Rayer.Core.Lyric.Impl;
using Rayer.Core.Menu;
using Rayer.Core.Utils;
using Rayer.SearchEngine.Events;
using Rayer.SearchEngine.Views.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Wpf.Ui.Appearance;

namespace Rayer.SearchEngine.ViewModels;

public partial class DynamicIslandViewModel : ObservableObject
{
    private readonly ILyricProvider _lyricProvider;
    private readonly IAudioManager _audioManager;
    private readonly DispatcherTimer _timer;

    private static readonly ILineInfo _noneLyricInfo = new LineInfo("暂无匹配歌词");
    private static readonly ILineInfo _pauseInfo = new LineInfo("暂停播放");
    private static readonly ILineInfo _stopInfo = new LineInfo("喵蛙王子丶");
    private static readonly ILineInfo _seekingInfo = new LineInfo("正在拖动");

    [ObservableProperty]
    private ImageSource? _cover;

    [ObservableProperty]
    private ILineInfo? _currentLine;

    private List<ILineInfo> _totalLines = [];
    private int _currentLineIndex = 0;

    public DynamicIslandViewModel(ILyricProvider lyricProvider, IAudioManager audioManager)
    {
        _lyricProvider = lyricProvider;
        _audioManager = audioManager;

        _lyricProvider.AudioPlaying += OnAudioPlaying;
        _lyricProvider.AudioChanged += OnAudioChanged;
        _lyricProvider.AudioPaused += OnAudioPaused;
        _lyricProvider.AudioStopped += OnAudioStopped;
        _lyricProvider.LyricChanged += OnLyricChanged;

        _audioManager.Playback.Seeked += OnSeeked;

        _timer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher)
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };
        _timer.Tick += OnTick;

        ContextMenu = AppCore.GetRequiredService<IContextMenuFactory>().CreateContextMenu(ContextMenuScope.DynamicIsland);

        ApplicationThemeManager.Changed += OnThemeChanged;
    }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        foreach (var item in ContextMenu.Items.SourceCollection)
        {
            if (item is MenuItem { Header: "切换歌词搜索器" } searcherMenu)
            {
                foreach (var searcherMenuItem in searcherMenu.Items)
                {
                    if (searcherMenuItem is MenuItem { Icon: not null } menuItem)
                    {
                        menuItem.Icon = ImageIconFactory.Create("Play", 18);
                    }
                }
            }

            
        }
    }

    public DynamicIsland DynamicIsland { get; set; } = default!;

    public ContextMenu ContextMenu { get; }

    private void OnAudioPlaying(object? sender, AudioPlayingArgs e)
    {
        if (e.PlaybackState is NAudio.Wave.PlaybackState.Paused && _totalLines.Count > 0)
        {
            CurrentLine = _totalLines[_currentLineIndex];

            _timer.Start();
        }
        else
        {
            CurrentLine = _stopInfo;
        }
    }

    private async void OnAudioChanged(object? sender, AudioChangedArgs e)
    {
        _timer.Stop();

        _totalLines.Clear();
        CurrentLine = new LineInfo(e.New.Title);
        Cover = e.New.Cover;

        var lyricData = _lyricProvider.LyricData;

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            if (lyricData is not null && lyricData.Lines is { Count: > 0 })
            {
                _totalLines = lyricData.Lines;
                _currentLineIndex = 0;
                CurrentLine = _totalLines[0];

                _timer.Start();
            }
            else
            {
                _totalLines.Clear();
                CurrentLine = _noneLyricInfo;

                Application.Current.Dispatcher.BeginInvoke(async () =>
                {
                    await Task.Delay(2000);

                    CurrentLine = _stopInfo;
                });
            }
        }, DispatcherPriority.Background);
    }

    private void OnAudioPaused(object? sender, EventArgs e)
    {
        CurrentLine = _pauseInfo;

        _timer.Stop();
    }

    private void OnAudioStopped(object? sender, EventArgs e)
    {
        CurrentLine = _stopInfo;
        Cover = null;
        _totalLines.Clear();

        _timer.Stop();
    }

    private void OnLyricChanged(object? sender, SwitchLyricSearcherArgs e)
    {
        var lyricData = _lyricProvider.LyricData;

        if (lyricData is not null && lyricData.Lines is { Count: > 0 })
        {
            _totalLines = lyricData.Lines;
            _currentLineIndex = 0;
            CurrentLine = _totalLines[0];

            OnSeeked(sender, e);
        }
        else
        {
            _totalLines = [];
            _currentLineIndex = 0;
            CurrentLine = _noneLyricInfo;
        }
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (CurrentLine is not null)
        {
            var currentLineIndex = _totalLines.IndexOf(CurrentLine);
            var nextLine = currentLineIndex == _totalLines.Count - 1 ? null : _totalLines[currentLineIndex + 1];

            if (nextLine is not null && _audioManager.Playback.CurrentTime.TotalMilliseconds >= nextLine.StartTime)
            {
                if (_currentLineIndex + 2 < _totalLines.Count &&
                    _totalLines[_currentLineIndex + 2].StartTime - nextLine.StartTime > 1000)
                {
                    DynamicIsland.TextBlurStroyboard.Begin();
                }

                CurrentLine = nextLine;
                _currentLineIndex++;
            }
        }
    }

    private void OnSeeked(object? sender, EventArgs e)
    {
        _timer.Stop();

        if (_audioManager.Playback.IsSeeking)
        {
            CurrentLine = _seekingInfo;

            return;
        }

        var currentTime = _audioManager.Playback.CurrentTime.TotalMilliseconds;

        var matched = false;
        for (var i = 0; i < _totalLines.Count; i++)
        {
            if (currentTime < _totalLines[i].StartTime)
            {
                if (i > 0)
                {
                    CurrentLine = _totalLines[i - 1];
                    _currentLineIndex = i - 1;
                }
                else
                {
                    CurrentLine = _totalLines[0];
                    _currentLineIndex = 0;
                }

                matched = true;
                break;
            }
        }

        if (!matched)
        {
            CurrentLine = _totalLines.LastOrDefault();
        }

        _timer.Start();
    }
}