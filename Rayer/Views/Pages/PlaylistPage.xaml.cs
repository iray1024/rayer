using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Controls;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Menu;
using Rayer.Core.Utils;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.ViewModels;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;
using ListViewItem = Rayer.Core.Controls.ListViewItem;

namespace Rayer.Views.Pages;

[Inject]
public partial class PlaylistPage : AdaptivePage, INavigableView<PlaylistPageViewModel>, INavigationAware
{
    private readonly IAudioManager _audioManager;
    private readonly IPlaylistService _playlistService;
    private readonly ICommandBinding _commandBinding;
    private readonly ISettingsService _settingsService;

    private int _hasNavigationTo = 0;

    public PlaylistPage(
        PlaylistPageViewModel viewModel,
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

        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public bool HasNavigationTo
    {
        get => _hasNavigationTo == 1;
        set => _ = Interlocked.Exchange(ref _hasNavigationTo, value ? 1 : 0);
    }

    public new PlaylistPageViewModel ViewModel { get; set; }

    protected override void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel ??= AppCore.GetRequiredService<PlaylistPageViewModel>();
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

    private void OnAudioChanged(object? sender, Core.Events.AudioChangedArgs e)
    {
        foreach (var listviewItem in LibListView.Items)
        {
            var vContainer = LibListView.ItemContainerGenerator.ContainerFromItem(listviewItem);

            if (vContainer is ListViewItem vItem)
            {
                if (vItem.DataContext is Audio audio)
                {
                    if (audio.Id == e.New.Id)
                    {
                        var navView = AppCore.GetRequiredService<INavigationService>().GetNavigationControl() as NavigationView;

                        if (navView?.Template.FindName("PART_NavigationViewContentPresenter", navView) is NavigationViewContentPresenter navPresenter)
                        {
                            var scrollViewer = ElementHelper.GetScrollViewer(navPresenter);

                            scrollViewer?.ScrollToTop();

                            var index = LibListView.Items.IndexOf(vItem.DataContext);

                            LibListView.SelectedIndex = index;
                            LibListView.ScrollIntoView(vItem.DataContext);
                            scrollViewer?.ScrollToVerticalOffset(56 * index);
                            vItem.IsSelected = true;
                        }
                    }
                    else
                    {
                        vItem.IsSelected = false;
                    }
                }
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

    private async void OnListViewItemDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
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

            if (item.IsVirualWebSource)
            {
                if (!_audioManager.Playback.TryGetAudio(item.Id, out var existsAudio))
                {
                    var provider = App.GetRequiredService<ISearchAudioEngineProvider>();
                    var engine = provider.GetAudioEngine(item.SearcherType);

                    var audio = await engine.GetAudioAsync(new SearchEngine.Core.Domain.Aduio.SearchAudioDetail
                    {
                        Id = item.Id,
                        Tags = item.Tags
                    });

                    item.Path = audio.Url;

                    if (item.Cover is null && item.CoverUri is not null)
                    {
                        item.Cover = await ImageSourceFactory.CreateWebSourceAsync(new Uri(item.CoverUri));
                    }
                }
                else
                {
                    item = existsAudio;
                }
            }

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
                //_audioManager.Playback.Queue.Clear();

                //foreach (var audio in ViewModel.Items)
                //{
                //    _audioManager.Playback.Queue.Add(audio);
                //}
            }

            await _audioManager.Playback.Play(item);
        }
    }

    private void OnListViewItemRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
                                Scope = ContextMenuScope.Playlist
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
                        else if (header == "移动到")
                        {
                            menuItem.Icon = ImageIconFactory.Create("AddTo", 18);

                            menuItem.Items.Clear();

                            foreach (var playlist in _playlistService.Playlists)
                            {
                                var vMenuItme = new MenuItem
                                {
                                    Header = playlist.Name,
                                    Command = _commandBinding.MoveToCommand,
                                    CommandParameter = new PlaylistUpdate
                                    {
                                        Id = ViewModel.Id,
                                        Target = audio,
                                        To = playlist.Id
                                    }
                                };

                                if (playlist.Audios.Contains(audio))
                                {
                                    vMenuItme.IsEnabled = false;
                                }

                                menuItem.Items.Add(vMenuItme);
                            }
                        }
                        else if (header == "删除")
                        {
                            menuItem.Icon = ImageIconFactory.Create("Recycle", 18);
                            menuItem.CommandParameter = new PlaylistUpdate
                            {
                                Id = ViewModel.Id,
                                Target = audio,
                            };
                        }
                    }
                }
            }
        }
    }

    public void OnNavigatedTo()
    {
        if (!HasNavigationTo)
        {
            HasNavigationTo = true;
        }
    }

    public void OnNavigatedFrom()
    {
        if (HasNavigationTo)
        {
            HasNavigationTo = false;
        }
    }

    private void OnAudioPresenterItemLoaded(object sender, RoutedEventArgs e)
    {
        if (_audioManager.Playback.Playing)
        {
            if (sender is AudioPresenter presenter &&
                presenter.DataContext is Audio audio)
            {
                if (audio.Id == _audioManager.Playback.Audio.Id)
                {
                    var index = LibListView.Items.IndexOf(presenter.DataContext);
                    LibListView.SelectedIndex = index;
                    LibListView.ScrollIntoView(presenter.DataContext);
                    presenter.IsSelected = true;
                }
                else
                {
                    presenter.IsSelected = false;
                }
            }
        }
    }
}