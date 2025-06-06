using NAudio.Wave;
using Rayer.Abstractions;
using Rayer.Controls.Adorners;
using Rayer.Core.Abstractions;
using Rayer.Core.PlayControl.Abstractions;
using Rayer.Core.Utils;
using Rayer.FrameworkCore;
using Rayer.Markup;
using Rayer.Services;
using Rayer.ViewModels;
using System.Threading.RateLimiting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using Wpf.Ui.Appearance;

namespace Rayer.Controls;

public partial class Playbar : UserControl
{
    private static PlaybarResource _resource;

    private readonly IThemeResourceProvider _themeResourceProvider;
    private readonly IAudioManager _audioManager;
    private readonly IPlaybarService _playbarService;
    private readonly IImmersivePlayerService _immersivePlayerService;

    private static Rect _bounds;
    private bool _isAdornerVisible = false;

    private readonly SlidingWindowRateLimiter _limiter = new(new SlidingWindowRateLimiterOptions
    {
        PermitLimit = 1,
        QueueLimit = 0,
        SegmentsPerWindow = 1,
        Window = TimeSpan.FromSeconds(1)
    });

    public Playbar()
    {
        var vm = App.GetRequiredService<PlaybarViewModel>();

        ViewModel = vm;
        DataContext = this;

        _themeResourceProvider = App.GetRequiredService<IThemeResourceProvider>();
        _audioManager = App.GetRequiredService<IAudioManager>();
        _playbarService = App.GetRequiredService<IPlaybarService>();
        _immersivePlayerService = App.GetRequiredService<IImmersivePlayerService>();

        ApplicationThemeManager.Changed += OnThemeChanged;

        _playbarService.PlayOrPauseTriggered += OnPlayOrPauseTriggered;
        _audioManager.AudioPlaying += OnAudioPlaying;
        _audioManager.AudioPaused += OnAudioPaused;
        _audioManager.AudioStopped += OnAudioStopped;

        _immersivePlayerService.Show += OnImmersivePlayerShow;
        _immersivePlayerService.Hidden += OnImmersivePlayerHidden;

        InitializeResource();

        InitializeComponent();

        _bounds = new Rect(0, 0, ActualWidth, ActualHeight);
    }

    private void OnAudioPlaying(object? sender, Core.Events.AudioPlayingArgs e)
    {
        var taskbar = AppCore.MainWindow.TaskbarItemInfo;

        if (taskbar is not null)
        {
            taskbar.ThumbButtonInfos[1].Description = "暂停";
            taskbar.ThumbButtonInfos[1].ImageSource = ImageSourceFactory.Create("pack://application:,,,/assets/dark/pause.png");

            foreach (var item in taskbar.ThumbButtonInfos)
            {
                item.IsEnabled = true;
            }
        }
    }

    private void OnAudioPaused(object? sender, EventArgs e)
    {
        var taskbar = AppCore.MainWindow.TaskbarItemInfo;

        if (taskbar is not null)
        {
            var item = AppCore.MainWindow.TaskbarItemInfo.ThumbButtonInfos[1];

            item.Description = "播放";
            item.ImageSource = ImageSourceFactory.Create("pack://application:,,,/assets/dark/play_24x24.png");
        }
    }

    private void OnAudioStopped(object? sender, EventArgs e)
    {
        ToggleControlsState(false);

        PlayOrPause.Source = _resource.Play;

        if (AppCore.MainWindow is not null &&
            AppCore.MainWindow.TaskbarItemInfo is TaskbarItemInfo taskbar)
        {
            foreach (var item in taskbar.ThumbButtonInfos)
            {
                item.IsEnabled = false;
            }

            taskbar.ThumbButtonInfos[1].Description = "播放";
            taskbar.ThumbButtonInfos[1].ImageSource = ImageSourceFactory.Create("pack://application:,,,/assets/dark/play_24x24.png");
        }
    }

    private void OnPlayOrPauseTriggered(object? sender, EventArgs e)
    {
        SetPlayOrPauseTheme();
    }

    public PlaybarViewModel ViewModel { get; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _bounds.Width = ActualWidth;
        _bounds.Height = ActualHeight;

        ToggleControlsState(false);

        var wnd = (MainWindow)App.GetRequiredService<IWindow>();
        wnd.SizeChanged += Wnd_SizeChanged;

        ViewModel.Playing += OnPlaybarPlaying;
    }

    private void InitializeResource()
    {
        _resource = _themeResourceProvider.GetPlaybarResource();
    }

    private void Wnd_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            ViewModel.ProgressWidth = (e.NewSize.Width - 400) / 2.0;

            _bounds.Width = e.NewSize.Width;
        }
    }

    #region Playbar Events   
    private void OnPlaybarPlaying(object? sender, EventArgs e)
    {
        ToggleControlsState(true);

        PlayOrPause.Source = _resource.Pause;
    }

    private void PlayOrPauseClick(object sender, MouseButtonEventArgs e)
    {
        _playbarService.PlayOrPause(true);

        SetPlayOrPauseTheme();
    }

    private async void PreviousClick(object sender, MouseButtonEventArgs e)
    {
        await _playbarService.Previous();
    }

    private async void NextClick(object sender, MouseButtonEventArgs e)
    {
        await _playbarService.Next();
    }
    #endregion

    #region Slider Events
    private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
    {
        ViewModel.IgnoreUpdateProgressValue = true;
        ViewModel.AudioManager.Playback.IsSeeking = true;
    }

    private void Slider_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
        ViewModel.AudioManager.Playback.Seek(ViewModel.ProgressValue);
    }

    private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
    {
        ViewModel.AudioManager.Playback.IsSeeking = false;

        ViewModel.AudioManager.Playback.Seek(ViewModel.ProgressValue);

        ViewModel.IgnoreUpdateProgressValue = false;
    }

    private void Slider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (ViewModel.AudioManager.Playback.DeviceManager.PlaybackState is PlaybackState.Paused)
        {
            PlayOrPause.Source = _resource.Pause;
            ViewModel.AudioManager.Playback.Resume();
        }

        var value = e.GetPosition(PlaybarSlider).X / PlaybarSlider.ActualWidth * (PlaybarSlider.Maximum - PlaybarSlider.Minimum);

        ViewModel.AudioManager.Playback.Seek(value);
    }
    #endregion    

    private void ToggleControlsState(bool enable = true)
    {
        Previous.IsEnabled = enable;
        PlayOrPause.IsEnabled = enable;
        Next.IsEnabled = enable;
        PlaybarSlider.IsEnabled = enable;
    }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        _resource = _themeResourceProvider.GetPlaybarResource();

        SetPlayOrPauseTheme();

        Previous.Source = _resource.Previous;
        Next.Source = _resource.Next;
    }

    private void SetPlayOrPauseTheme()
    {
        var state = _playbarService.PlaybackState;

        PlayOrPause.Source = state is PlaybackState.Playing
            ? _resource.Pause
            : _resource.Play;
    }

    private void OnMouseEnter(object sender, MouseEventArgs e)
    {
        if (_isAdornerVisible)
        {
            return;
        }

        _isAdornerVisible = true;

        AddControllerAdorner();
        AddPlayQueueAdorner();
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        if (_bounds.Contains(e.GetPosition(this)))
        {
            return;
        }

        RemoveControllerAdnoner();
        RemovePlayQueueAdnoner();

        _isAdornerVisible = false;
    }

    private void AddControllerAdorner()
    {
        var element = (UIElement)Controller;

        var adornerLayer = AdornerLayer.GetAdornerLayer(element);
        var adnoners = adornerLayer.GetAdorners(element);

        if (adornerLayer is not null && (adnoners is null || adnoners.Length == 0))
        {
            adornerLayer.Add(new PlayloopAdorner(element));
            adornerLayer.Add(new VolumeAdorner(element));
            adornerLayer.Add(new SpeedAdorner(element));
            adornerLayer.Add(new PitchAdorner(element));
        }
    }

    private void RemoveControllerAdnoner()
    {
        var element = (UIElement)Controller;

        var adornerLayer = AdornerLayer.GetAdornerLayer(element);
        var adorners = adornerLayer.GetAdorners(element);

        if (adorners is not null && adorners.Length > 0)
        {
            foreach (var adorner in adorners)
            {
                adornerLayer.Remove(adorner);
            }
        }
    }

    private void AddPlayQueueAdorner()
    {
        var element = (UIElement)PlayQueue;

        var adornerLayer = AdornerLayer.GetAdornerLayer(element);
        var adnoners = adornerLayer.GetAdorners(element);

        if (adornerLayer is not null && (adnoners is null || adnoners.Length == 0))
        {
            adornerLayer.Add(new EqualizerAdorner(element));
        }
    }

    private void RemovePlayQueueAdnoner()
    {
        var element = (UIElement)PlayQueue;

        var adornerLayer = AdornerLayer.GetAdornerLayer(element);
        var adorners = adornerLayer.GetAdorners(element);

        if (adorners is not null && adorners.Length > 0)
        {
            foreach (var adorner in adorners)
            {
                adornerLayer.Remove(adorner);
            }
        }
    }

    private async void OnAlbumMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        using var lease = await _limiter.AcquireAsync();

        if (lease.IsAcquired)
        {
            await App.GetRequiredService<IImmersivePlayerService>().ToggleShow();
        }
    }

    private void OnAlbumMouseEnter(object sender, MouseEventArgs e)
    {
        FullScreen.Source = (ImageSource)Application.Current.Resources[nameof(ThemeSymbol.FullScreen)];

        AlbumMask.Visibility = Visibility.Visible;
    }

    private void OnAlbumMouseLeave(object sender, MouseEventArgs e)
    {
        AlbumMask.Visibility ^= Visibility.Collapsed;
    }

    private void OnImmersivePlayerShow(object? sender, EventArgs e)
    {
        Title.Foreground = StaticThemeResources.Dark.TextFillColorPrimaryBrush;
        Artists.Foreground = StaticThemeResources.Dark.TextFillColorPrimaryBrush;

        Previous.Source = StaticThemeResources.Dark.Previous;

        PlayOrPause.Source = _playbarService.PlaybackState is PlaybackState.Playing
            ? StaticThemeResources.Dark.Pause
            : StaticThemeResources.Dark.Play;

        Next.Source = StaticThemeResources.Dark.Next;

        PlayQueue.PlayQueue.Source = StaticThemeResources.Dark.PlayQueue;

        CurrentTime.Foreground = StaticThemeResources.Dark.TextFillColorPrimaryBrush;
        TotalTime.Foreground = StaticThemeResources.Dark.TextFillColorPrimaryBrush;

        var trackBorder = (Border)PlaybarSlider.Template.FindName("TrackBackground", PlaybarSlider);
        trackBorder.Background = StaticThemeResources.Dark.SliderTrackFill;

        var thumb = (Thumb)PlaybarSlider.Template.FindName("Thumb", PlaybarSlider);
        thumb.Foreground = StaticThemeResources.Dark.SliderThumbForeground;
        thumb.Background = StaticThemeResources.Dark.SliderThumbBackground;
    }

    private void OnImmersivePlayerHidden(object? sender, EventArgs e)
    {
        Title.SetResourceReference(ForegroundProperty, "TextFillColorPrimaryBrush");
        Artists.SetResourceReference(ForegroundProperty, "TextFillColorPrimaryBrush");

        Previous.Source = (ImageSource)Application.Current.Resources[nameof(ThemeSymbol.Previous)];

        PlayOrPause.Source = _playbarService.PlaybackState is PlaybackState.Playing
            ? (ImageSource)Application.Current.Resources[nameof(ThemeSymbol.Pause)]
            : (ImageSource)Application.Current.Resources[nameof(ThemeSymbol.Play)];

        Next.Source = (ImageSource)Application.Current.Resources[nameof(ThemeSymbol.Next)];

        PlayQueue.PlayQueue.Source = (ImageSource)StaticThemeResources.GetDynamicResource(nameof(ThemeSymbol.PlayQueue));

        CurrentTime.SetResourceReference(ForegroundProperty, "TextFillColorPrimaryBrush");
        TotalTime.SetResourceReference(ForegroundProperty, "TextFillColorPrimaryBrush");

        var trackBorder = (Border)PlaybarSlider.Template.FindName("TrackBackground", PlaybarSlider);
        trackBorder.SetResourceReference(BackgroundProperty, "SliderTrackFill");

        var thumb = (Thumb)PlaybarSlider.Template.FindName("Thumb", PlaybarSlider);
        thumb.SetResourceReference(ForegroundProperty, "SliderThumbBackground");
        thumb.SetResourceReference(BackgroundProperty, "SliderOuterThumbBackground");
    }
}