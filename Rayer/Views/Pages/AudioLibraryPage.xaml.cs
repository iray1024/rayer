using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Controls;
using Rayer.Core.Events;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Menu;
using Rayer.Core.Utils;
using Rayer.ViewModels;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui;
using Wpf.Ui.Controls;
using ListViewItem = Rayer.Core.Controls.ListViewItem;

namespace Rayer.Views.Pages;

[Inject]
public partial class AudioLibraryPage : AdaptivePage, INavigableView<AudioLibraryViewModel>
{
    private readonly IAudioManager _audioManager;
    private readonly IPlaylistService _playlistService;
    private readonly ICommandBinding _commandBinding;
    private readonly ISettingsService _settingsService;

    public AudioLibraryPage(
        AudioLibraryViewModel viewModel,
        IAudioManager audioManager,
        IPlaylistService playlistService,
        ICommandBinding commandBinding,
        ISettingsService settingsService)
        : base(viewModel)
    {
        _audioManager = audioManager;
        _playlistService = playlistService;
        _commandBinding = commandBinding;
        _settingsService = settingsService;

        _audioManager.AudioChanged += OnAudioChanged;
        _audioManager.AudioStopped += OnAudioStopped;
        _audioManager.Audios.CollectionChanged += AudioCollectionChanged;

        ViewModel = viewModel;
        DataContext = this;

        ViewModel.Items.AddRange(_audioManager.Audios);

        InitializeComponent();
    }

    public new AudioLibraryViewModel ViewModel { get; private set; }

    protected override void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel ??= AppCore.GetRequiredService<AudioLibraryViewModel>();
        base.ViewModel = ViewModel;

        base.OnLoaded(sender, e);

        if (_audioManager.Playback.Audio is Audio audio && _audioManager.Playback.Playing)
        {
            var navView = AppCore.GetRequiredService<INavigationService>().GetNavigationControl() as NavigationView;

            if (navView?.Template.FindName("PART_NavigationViewContentPresenter", navView) is NavigationViewContentPresenter navPresenter)
            {
                var scrollViewer = ElementHelper.GetScrollViewer(navPresenter);

                scrollViewer?.ScrollToTop();

                var innerAudio = LibListView.Items.IndexOf(audio);

                if (innerAudio != -1)
                {
                    scrollViewer?.ScrollToVerticalOffset(56 * innerAudio);
                }
            }
        }
    }

    protected override void OnUnloaded(object sender, RoutedEventArgs e)
    {
        base.OnUnloaded(sender, e);
    }

    private void OnAudioChanged(object? sender, AudioChangedArgs e)
    {
        var index = ViewModel.Items.IndexOf(e.New);
        LibListView.SelectedIndex = index;
        LibListView.ScrollIntoView(e.New);

        foreach (var listviewItem in LibListView.Items)
        {
            var vContainer = LibListView.ItemContainerGenerator.ContainerFromItem(listviewItem);

            if (vContainer is ListViewItem vItem)
            {
                vItem.IsSelected = vItem.DataContext.Equals(e.New);
            }
        }
    }

    private void OnAudioStopped(object? sender, EventArgs e)
    {
        LibListView.SelectedIndex = -1;
        foreach (var listviewItem in LibListView.Items)
        {
            var vContainer = LibListView.ItemContainerGenerator.ContainerFromItem(listviewItem);

            if (vContainer is ListViewItem vItem)
            {
                vItem.IsSelected = false;
            }
        }
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
                        var audio = (Audio)item;

                        ViewModel.Items.Insert(startIndex++, audio);
                        _audioManager.Playback.Queue.Add(audio);
                    }
                }
            }

            if (e.Action is NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems is not null &&
                    e.OldItems.Count > 0 &&
                    e.OldItems[0] is Audio audio)
                {
                    ViewModel.Items.Remove(audio);
                    _audioManager.Playback.Queue.Remove(audio);
                }
            }
        });
    }

    private async void OnListViewItemDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is ListViewItem listViewItem &&
            listViewItem.DataContext is Audio item)
        {
            foreach (var listviewItem in LibListView.Items)
            {
                var vContainer = LibListView.ItemContainerGenerator.ContainerFromItem(listviewItem);

                if (vContainer is ListViewItem vItem)
                {
                    vItem.IsSelected = false;
                }
            }

            listViewItem.IsSelected = true;

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
                    if (menuItem.Header is string header)
                    {
                        if (header == "播放")
                        {
                            menuItem.Icon = ImageIconFactory.Create("Play", 18);

                            menuItem.CommandParameter = new AudioCommandParameter()
                            {
                                Audio = audio,
                                Scope = ContextMenuScope.Library
                            };
                        }
                        else if (header == "添加到")
                        {
                            menuItem.Icon = ImageIconFactory.Create("AddTo", 18);

                            menuItem.Items.Clear();

                            foreach (var playlist in _playlistService.Playlists)
                            {
                                var vMenuItme = new MenuItem
                                {
                                    Header = playlist.Name,
                                    Command = _commandBinding.AddToCommand,
                                    CommandParameter = new PlaylistUpdate
                                    {
                                        Id = playlist.Id,
                                        Target = audio
                                    }
                                };

                                if (playlist.Audios.Contains(audio))
                                {
                                    vMenuItme.IsEnabled = false;
                                }

                                menuItem.Items.Add(vMenuItme);
                            }
                        }
                    }
                }
            }
        }
    }
}