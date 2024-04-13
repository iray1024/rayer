using Rayer.Command.Parameter;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Events;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Utils;
using Rayer.ViewModels;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace Rayer.Views.Pages;

[Inject]
public partial class AudioLibraryPage : INavigableView<AudioLibraryViewModel>
{
    private readonly IAudioManager _audioManager;
    private readonly ISettingsService _settingsService;

    public AudioLibraryPage(
        AudioLibraryViewModel viewModel,
        IAudioManager audioManager,
        ISettingsService settingsService)
    {
        _audioManager = audioManager;
        _settingsService = settingsService;

        _audioManager.AudioChanged += OnAudioChanged;
        _audioManager.AudioStopped += OnAudioStopped;
        _audioManager.Audios.CollectionChanged += AudioCollectionChanged;

        ViewModel = viewModel;
        DataContext = this;

        ViewModel.Items.AddRange(_audioManager.Audios);

        InitializeComponent();
    }

    private void OnAudioChanged(object? sender, AudioChangedArgs e)
    {
        var index = ViewModel.Items.IndexOf(e.New);
        LibListView.SelectedIndex = index;
        LibListView.ScrollIntoView(e.New);
    }

    private void OnAudioStopped(object? sender, EventArgs e)
    {
        LibListView.SelectedIndex = -1;
    }

    private void AudioCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (e.Action is NotifyCollectionChangedAction.Add)
            {
                var startIndex = e.NewStartingIndex;
                if (e.NewItems is not null)
                {
                    foreach (var item in e.NewItems)
                    {
                        ViewModel.Items.Insert(startIndex++, (Audio)item);
                    }
                }
            }

            if (e.Action is NotifyCollectionChangedAction.Remove)
            {
                ViewModel.Items.RemoveAt(e.OldStartingIndex);
            }
        });
    }

    public AudioLibraryViewModel ViewModel { get; }

    private async void OnListViewItemDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is ListViewItem listViewItem &&
            listViewItem.DataContext is Audio item)
        {
            if (_settingsService.Settings.PlaySingleAudioStrategy is PlaySingleAudioStrategy.AddToQueue)
            {
                var index = _audioManager.Playback.Queue.IndexOf(item);

                if (index == -1)
                {
                    _audioManager.Playback.Queue.Add(item);
                }
            }
            else
            {
                _audioManager.Playback.Queue.Clear();
                // 后续实现歌单后要修改逻辑
                foreach (var audio in _audioManager.Audios)
                {
                    _audioManager.Playback.Queue.Add(audio);
                }
            }

            await _audioManager.Playback.Play(item);
        }
    }

    private void OnListViewItemRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        foreach (var item in ViewModel.ContextMenu.Items.SourceCollection)
        {
            if (item is System.Windows.Controls.MenuItem menuItem)
            {
                if (sender is FrameworkElement { DataContext: Audio audio })
                {
                    menuItem.CommandParameter = new AudioCommandParameter()
                    {
                        Audio = audio,
                        Scope = ContextMenuScope.Library
                    };
                }

                if (menuItem.Header is string header)
                {
                    if (header == "播放")
                    {
                        menuItem.Icon = ImageIconFactory.Create("Play", 18);
                    }
                    else if (header == "添加到")
                    {
                        menuItem.Icon = ImageIconFactory.Create("AddTo", 18);
                    }
                }
            }
        }
    }
}