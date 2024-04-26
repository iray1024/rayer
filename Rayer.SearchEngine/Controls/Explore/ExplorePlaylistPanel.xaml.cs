using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Controls;
using Rayer.Core.Events;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Album;
using Rayer.SearchEngine.Core.Domain.Playlist;
using Rayer.SearchEngine.Core.Enums;
using Rayer.SearchEngine.ViewModels.Explore.Playlist;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Wpf.Ui;
using ListViewItem = Rayer.Core.Controls.ListViewItem;

namespace Rayer.SearchEngine.Controls.Explore;

[Inject]
public partial class ExplorePlaylistPanel : AdaptiveUserControl
{
    private readonly IAudioManager _audioManager;
    private readonly IMemoryCache _cache;
    private readonly IContentDialogService _contentDialogService;

    public ExplorePlaylistPanel(ExplorePlaylistPancelViewModel vm)
        : base(vm)
    {
        ViewModel = vm;
        DataContext = this;

        _cache = AppCore.GetRequiredService<IMemoryCache>();
        _contentDialogService = AppCore.GetRequiredService<IContentDialogService>();

        _audioManager = AppCore.GetRequiredService<IAudioManager>();

        _audioManager.AudioChanged += OnAudioChanged;
        _audioManager.AudioStopped += OnAudioStopped;

        InitializeComponent();
    }

    public new ExplorePlaylistPancelViewModel ViewModel { get; set; }

    protected override async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is PlaylistDetail detail)
        {
            var cacheKey = detail.GetHashCode();

            if (!_cache.TryGetValue<PlaylistDetail>(cacheKey, out var playlistDetail) || playlistDetail is { AudioCount: > 0, Audios.Length: 0 })
            {
                await Task.Run(async () =>
                {
                    if (detail.AudioCount > 0 && detail.Audios.Length == 0)
                    {
                        var provider = AppCore.GetRequiredService<IAggregationServiceProvider>();

                        playlistDetail = await provider.PlaylistService.GetPlaylistDetailAsync(detail.Id);

                        _cache.Set(cacheKey, playlistDetail, TimeSpan.FromMinutes(1));
                    }
                });
            }

            ViewModel.Detail = playlistDetail ?? detail;
        }
        else if (DataContext is Album album)
        {
            var cacheKey = album.GetHashCode();

            if (!_cache.TryGetValue<Album>(cacheKey, out var albumDetail) || albumDetail is { AudioCount: > 0, Audios.Length: 0 })
            {
                await Task.Run(async () =>
                {
                    if (album.AudioCount > 0 && album.Audios.Length == 0)
                    {
                        var provider = AppCore.GetRequiredService<IAggregationServiceProvider>();

                        albumDetail = await provider.AlbumEngine.SearchAlbumDetailAsync(album.Id);

                        _cache.Set(cacheKey, albumDetail, TimeSpan.FromMinutes(1));
                    }
                });
            }

            var mapper = AppCore.GetRequiredService<IMapper>();

            var mapDetail = mapper.Map<PlaylistDetail>(albumDetail);
            ViewModel.Detail = mapDetail;
        }
        else if (DataContext is SearchAlbumDetail searchAlbum)
        {
            var cacheKey = searchAlbum.GetHashCode();

            if (!_cache.TryGetValue<Album>(cacheKey, out var searchAlbumDetail) || searchAlbumDetail is { AudioCount: > 0, Audios.Length: 0 })
            {
                await Task.Run(async () =>
                {
                    if (searchAlbum.AudioCount > 0 && searchAlbum.Audios.Length == 0)
                    {
                        var provider = AppCore.GetRequiredService<IAggregationServiceProvider>();

                        searchAlbumDetail = await provider.AlbumEngine.SearchAlbumDetailAsync(searchAlbum.Id);

                        _cache.Set(cacheKey, searchAlbumDetail, TimeSpan.FromMinutes(1));
                    }
                });
            }

            var mapper = AppCore.GetRequiredService<IMapper>();

            var mapDetail = mapper.Map<PlaylistDetail>(searchAlbumDetail);
            ViewModel.Detail = mapDetail;
        }

        DataContext = this;

        AlbumHeader.Visibility = Visibility.Visible;

        base.OnLoaded(sender, e);
    }

    protected override void OnUnloaded(object sender, RoutedEventArgs e)
    {
        AlbumHeader.Visibility = Visibility.Collapsed;

        base.OnUnloaded(sender, e);
    }

    protected override void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        base.OnSizeChanged(sender, e);

        var panelWidth = (int)AppCore.MainWindow.ActualWidth - 666;

        ViewModel.DescriptionMaxWidth = panelWidth;
    }

    private void OnAudioChanged(object? sender, AudioChangedArgs e)
    {
        foreach (var listviewItem in LibListView.Items)
        {
            var vContainer = LibListView.ItemContainerGenerator.ContainerFromItem(listviewItem);

            if (vContainer is ListViewItem vItem)
            {
                if (vItem.DataContext is SearchAudioDetail detail)
                {
                    if (detail.Id == e.New.Id)
                    {
                        var index = LibListView.Items.IndexOf(vItem.DataContext);
                        LibListView.SelectedIndex = index;
                        LibListView.ScrollIntoView(vItem.DataContext);
                        vItem.IsSelected = true;
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

    private void OnListViewItemRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }

    private async void OnListViewItemDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.Source is ListViewItem listViewItem &&
            listViewItem.DataContext is SearchAudioDetail item)
        {
            if (item.Copyright.HasCopyright)
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

                await ViewModel.PlayWebAudio(item);
            }            
        }
    }

    private void OnListViewLoaded(object sender, RoutedEventArgs e)
    {
        AppCore.GetRequiredService<ILoaderProvider>().Loaded();
        ContainerPanel.Visibility = Visibility.Visible;
    }

    #region Effect
    private void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is AsyncImage cover)
        {
            var center = 256 >> 1;

            var transform = new ScaleTransform()
            {
                CenterX = center,
                CenterY = center
            };

            cover.RenderTransform = transform;

            var animationX = new DoubleAnimation
            {
                To = 1.2,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase()
            };

            var animationY = new DoubleAnimation
            {
                To = 1.2,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase()
            };

            transform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
        }
    }

    private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is AsyncImage cover)
        {
            var center = 256 >> 1;

            var transform = new ScaleTransform()
            {
                CenterX = center,
                CenterY = center
            };
            cover.RenderTransform = transform;

            var animationX = new DoubleAnimation
            {
                From = 1.2,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase()
            };

            var animationY = new DoubleAnimation
            {
                From = 1.2,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new QuadraticEase()
            };

            transform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
        }
    }
    #endregion

    #region DescriptionStoryboard
    private static readonly Storyboard _descriptionStoryboard = new();

    private void OnDescriptionMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        _descriptionStoryboard.Stop();
        _descriptionStoryboard.Children.Clear();

        var animation = new DoubleAnimation()
        {
            From = 0.8,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(200),
            EasingFunction = new CubicEase()
        };

        Storyboard.SetTarget(animation, DescriptionPanel);
        Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));

        _descriptionStoryboard.Children.Add(animation);

        _descriptionStoryboard.Begin();
    }

    private void OnDescriptionMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        _descriptionStoryboard.Stop();
        _descriptionStoryboard.Children.Clear();

        var animation = new DoubleAnimation()
        {
            From = 1,
            To = 0.8,
            Duration = TimeSpan.FromMilliseconds(200),
            EasingFunction = new CubicEase()
        };

        Storyboard.SetTarget(animation, DescriptionPanel);
        Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));

        _descriptionStoryboard.Children.Add(animation);

        _descriptionStoryboard.Begin();
    }
    #endregion

    private async void OnDescriptionMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var type = ViewModel.Detail.Type is SearchType.Audio
            ? "单曲"
            : ViewModel.Detail.Type is SearchType.Album
                ? "专辑"
                : ViewModel.Detail.Type is SearchType.Playlist
                    ? "歌单"
                    : string.Empty;

        var dialog = new MaskContentDialog(_contentDialogService.GetDialogHost())
        {
            Title = $"{type}介绍",
            IsFooterVisible = false
        };

        MaskContentDialog.SetDescription(dialog, ViewModel.Detail.Description ?? string.Empty);

        await dialog.ShowAsync();
    }

    private void OnAudioPresenterItemLoaded(object sender, RoutedEventArgs e)
    {
        if (_audioManager.Playback.Playing)
        {
            if (sender is AudioPresenter presenter &&
                presenter.DataContext is SearchAudioDetail detail)
            {
                if (detail.Id == _audioManager.Playback.Audio.Id)
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