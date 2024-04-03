using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Events;
using Rayer.Core.Extensions;
using Rayer.Core.Models;
using Rayer.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace Rayer.Views.Pages;

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

        viewModel.Items.Source = new ObservableCollection<Audio>(_audioManager.Audios);

        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    private void OnAudioChanged(object? sender, AudioChangedArgs e)
    {
        if (ViewModel.Items.Source is Collection<Audio>)
        {
            var index = ViewModel.Items.View.IndexOf(e.New);
            LibListView.SelectedIndex = index;
            LibListView.ScrollIntoView(LibListView.Items[index]);
        }
    }

    private void OnAudioStopped(object? sender, EventArgs e)
    {
        LibListView.SelectedIndex = -1;
    }

    private void AudioCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (ViewModel.Items.Source is Collection<Audio> audios)
            {
                if (e.Action is NotifyCollectionChangedAction.Add)
                {
                    var startIndex = e.NewStartingIndex;
                    if (e.NewItems is not null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            audios.Insert(startIndex++, (Audio)item);
                        }
                    }
                }

                if (e.Action is NotifyCollectionChangedAction.Remove)
                {
                    audios.RemoveAt(e.OldStartingIndex);
                }
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
                foreach (var audio in _audioManager.Audios)
                {
                    if (_audioManager.Playback.Queue.IndexOf(audio) == -1)
                    {
                        _audioManager.Playback.Queue.Add(audio);
                    }
                }
            }

            await _audioManager.Playback.Play(item);
        }
    }
}