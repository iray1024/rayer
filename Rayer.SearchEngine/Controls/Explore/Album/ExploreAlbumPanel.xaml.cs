using Microsoft.Extensions.Caching.Memory;
using Rayer.Core;
using Rayer.Core.Controls;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Core.Domain.Playlist;
using Rayer.SearchEngine.ViewModels.Explore.Album;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Rayer.SearchEngine.Controls.Explore.Album;

[Inject]
public partial class ExploreAlbumPanel : AdaptiveUserControl
{
    private readonly IMemoryCache _cache;

    public ExploreAlbumPanel(ExploreAlbumPanelViewModel vm)
        : base(vm)
    {
        ViewModel = vm;
        DataContext = this;

        _cache = AppCore.GetRequiredService<IMemoryCache>();

        InitializeComponent();
    }

    public new ExploreAlbumPanelViewModel ViewModel { get; set; }

    protected override async void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel ??= AppCore.GetRequiredService<ExploreAlbumPanelViewModel>();
        base.ViewModel = ViewModel;

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

            DataContext = this;
        }

        base.OnLoaded(sender, e);
    }

    protected override void OnUnloaded(object sender, RoutedEventArgs e)
    {
        base.OnUnloaded(sender, e);

        ViewModel = default!;

        BindingOperations.ClearAllBindings(this);

        GC.Collect();
    }

    private void OnListViewItemRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }

    private async void OnListViewItemDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.Source is ListViewItem listViewItem &&
            listViewItem.DataContext is SearchAudioDetail item)
        {
            await ViewModel.PlayWebAudio(item);
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
        if (sender is Wpf.Ui.Controls.Image cover)
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
        if (sender is Wpf.Ui.Controls.Image cover)
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
}