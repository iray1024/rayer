using Rayer.Core.Abstractions;
using Rayer.Core.Events;
using Rayer.Core.Models;
using Rayer.ViewModels;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace Rayer.Views.Pages;

public partial class AudioLibraryPage : INavigableView<AudioLibraryViewModel>
{
    private readonly IAudioManager _audioManager;

    public AudioLibraryPage(
        AudioLibraryViewModel viewModel,
        IAudioManager audioManager)
    {
        _audioManager = audioManager;

        _audioManager.AudioChanged += AudioChanged;
        _audioManager.Audios.CollectionChanged += AudioCollectionChanged;

        viewModel.Items = [.. _audioManager.Audios];

        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    private void AudioChanged(object? sender, AudioChangedArgs e)
    {
        LibListView.SelectedIndex = ViewModel.Items.IndexOf(e.New);
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
            var index = _audioManager.Playback.Queue.IndexOf(item);

            if (index == -1)
            {
                _audioManager.Playback.Queue.Add(item);
            }

            await _audioManager.Playback.Play(item);
        }
    }
}