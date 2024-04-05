using Rayer.Command.Parameter;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Events;
using Rayer.Core.Extensions;
using Rayer.Core.Models;
using Rayer.Core.Utils;
using Rayer.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace Rayer.Controls;

public partial class RightPlaybarPanel : UserControl
{
    private readonly IAudioManager _audioManager;

    public RightPlaybarPanel()
    {
        var vm = App.GetRequiredService<RightPlaybarPanelViewModel>();

        ViewModel = vm;
        DataContext = this;

        _audioManager = App.GetRequiredService<IAudioManager>();

        _audioManager.AudioChanged += AudioChanged;
        _audioManager.Audios.CollectionChanged += OnAudiosCollectionChanged;
        _audioManager.Playback.Queue.CollectionChanged += OnPlayQueueCollectionChanged;

        ViewModel.Items.Source = new Collection<Audio>([.. _audioManager.Playback.Queue]);

        ViewModel.QueueCount = $"共{(ViewModel.Items.Source as ICollection<Audio>)?.Count}首歌曲";

        ApplicationThemeManager.Changed += ThemeChanged;

        InitializeComponent();
    }

    public RightPlaybarPanelViewModel ViewModel { get; set; }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {        
        ViewModel.OnButtonClick();

        if (ViewModel.Items.Source is Collection<Audio>)
        {            
            LibListView.ScrollIntoView(_audioManager.Playback.Audio);
        }
    }

    private void OnRecycleMouseUp(object sender, MouseButtonEventArgs e)
    {
        ViewModel.Items.Source = new Collection<Audio>();
        ViewModel.QueueCount = $"共0首歌曲";

        if (_audioManager.Playback.Playing)
        {
            _audioManager.Playback.StopPlay();
        }

        _audioManager.Playback.Queue.Clear();
    }

    private void AudioChanged(object? sender, AudioChangedArgs e)
    {
        if (ViewModel.Items.Source is Collection<Audio>)
        {
            var index = ViewModel.Items.View.IndexOf(e.New);
            LibListView.SelectedIndex = index;
            LibListView.ScrollIntoView(e.New);
        }
    }

    private void OnAudiosCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
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
                    if (audios.Count > e.OldStartingIndex)
                    {
                        audios.RemoveAt(e.OldStartingIndex);
                    }
                }

                ViewModel.QueueCount = $"共{audios.Count}首歌曲";
            }
        });
    }

    private void OnPlayQueueCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
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

                ViewModel.Items.View.Refresh();
                ViewModel.QueueCount = $"共{audios.Count}首歌曲";
            }
        });
    }

    private async void OnListViewItemDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is ListViewItem listViewItem &&
            listViewItem.DataContext is Audio item)
        {
            await ViewModel.Play(item);
        }
    }

    private void ThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        PlayQueue.Source = (ImageSource)Application.Current.Resources["PlayQueue"];
        Recycle.Source = (ImageSource)Application.Current.Resources["Recycle"];
    }

    private void OnListViewItemRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        foreach (var item in ViewModel.ContextMenu.Items.SourceCollection)
        {
            if (item is MenuItem menuItem)
            {
                if (sender is FrameworkElement { DataContext: Audio audio })
                {
                    menuItem.CommandParameter = new AudioCommandParameter()
                    {
                        Audio = audio,
                        Scope = ContextMenuScope.PlayQueue
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
                    else if (header == "删除")
                    {
                        menuItem.Icon = ImageIconFactory.Create("Recycle", 18);
                    }
                }
            }
        }
    }
}