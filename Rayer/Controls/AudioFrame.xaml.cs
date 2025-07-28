using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Controls;
using Rayer.Core.Menu;
using Rayer.FrameworkCore;
using Rayer.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rayer.Controls;

public partial class AudioFrame : UserControl
{
    #region 依赖属性   
    public static readonly DependencyProperty AlbumProperty =
        DependencyProperty.Register(
            nameof(Album),
            typeof(ImageSource),
            typeof(AudioFrame),
            new UIPropertyMetadata(StaticThemeResources.AlbumFallback));

    public static readonly DependencyProperty MeidaSourceProperty =
       DependencyProperty.Register(
           nameof(MeidaSource),
           typeof(string),
           typeof(AudioFrame),
           new UIPropertyMetadata(null, OnMediaSourceChanged));

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(AudioFrame),
            new PropertyMetadata());

    public static readonly DependencyProperty InformationProperty =
        DependencyProperty.Register(
            nameof(Information),
            typeof(string),
            typeof(AudioFrame),
            new PropertyMetadata());

    public static readonly DependencyProperty CurrentMarginProperty =
        DependencyProperty.Register(
            nameof(CurrentMargin),
            typeof(Thickness),
            typeof(AudioFrame),
            new PropertyMetadata(new Thickness(0, 96, 0, 140)));
    #endregion

    private readonly IAudioManager audioManager;
    private readonly ICoverManager coverManager;
    private readonly IContextMenuFactory contextMenuFactory;

    private static Border _thisBorder = default!;
    private static MediaElement? _media;

    public AudioFrame()
    {
        DataContext = this;

        InitializeComponent();

        audioManager = AppCore.GetRequiredService<IAudioManager>();
        audioManager.AudioChanged += OnAudioChanged;
        audioManager.AudioStopped += OnAudioStopped;

        coverManager = AppCore.GetRequiredService<ICoverManager>();
        coverManager.CoverChanged += OnCoverChanged;

        contextMenuFactory = AppCore.GetRequiredService<IContextMenuFactory>();

        UpdateAudioMetadata(audioManager.Playback.Audio);
    }

    private void OnAudioChanged(object? sender, Core.Events.AudioChangedArgs e)
    {
        UpdateAudioMetadata(e.New);
    }

    private void OnAudioStopped(object? sender, EventArgs e)
    {
        UpdateAudioMetadata(null);
    }

    private void OnCoverChanged(object? sender, Audio e)
    {
        if (audioManager.Playback.Audio.Equals(e))
        {
            MeidaSource = coverManager.GetCover(e);
        }
    }

    public ImageSource Album
    {
        get => (ImageSource)GetValue(AlbumProperty);
        set => SetValue(AlbumProperty, value);
    }

    public string? MeidaSource
    {
        get => (string)GetValue(MeidaSourceProperty);
        set => SetValue(MeidaSourceProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Information
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(InformationProperty, value);
    }

    public Thickness CurrentMargin
    {
        get => (Thickness)GetValue(CurrentMarginProperty);
        set => SetValue(CurrentMarginProperty, value);
    }

    private void UpdateAudioMetadata(Audio? audio)
    {
        if (audio is null)
        {
            Album = StaticThemeResources.AlbumFallback;
            Title = string.Empty;
            Information = string.Empty;
            MeidaSource = null;

            return;
        }

        Album = audio.Cover ?? StaticThemeResources.AlbumFallback;
        Title = audio.Title;
        Information = $"{string.Join('&', audio.Artists)} - {audio.Album}";
        MeidaSource = coverManager.GetCover(audio);

        ImagePresenter.ContextMenu = contextMenuFactory.CreateContextMenu(ContextMenuScope.AlbumPresenter, audio);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged += OnSizeChanged;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged -= OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var width = Math.Max(300, e.NewSize.Width / 3);
        var currentWindowHeight = AppCore.MainWindow.ActualHeight;
        var currentTopMargin = Math.Max(0, ((currentWindowHeight - width) / 4) + 16);

        CurrentMargin = new Thickness(0, currentTopMargin, 0, 140);
    }

    private static async void OnMediaSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AudioFrame { Content: Border border })
        {
            _thisBorder = border;
            var videoPath = e.NewValue as string;

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (!string.IsNullOrWhiteSpace(videoPath))
                {
                    // 创建 MediaElement 用于播放视频
                    var mediaElement = new MediaElement
                    {
                        Source = new Uri(videoPath),
                        IsMuted = true,
                        SpeedRatio = 0.6,
                        LoadedBehavior = MediaState.Manual,
                        UnloadedBehavior = MediaState.Manual,
                    };

                    mediaElement.MediaOpened += (s, e) => mediaElement.Clip = new RectangleGeometry(new Rect(0, 0, mediaElement.NaturalVideoWidth, mediaElement.NaturalVideoHeight), 38, 38);
                    mediaElement.MediaEnded += (s, e) => mediaElement.Position = TimeSpan.Zero;

                    mediaElement.Play();

                    _media = mediaElement;
                    var audioManager = AppCore.GetRequiredService<IAudioManager>();
                    audioManager.Playback.AudioPaused += OnMediaAudioPaused;
                    audioManager.Playback.AudioPlaying += OnMediaAudioPlaying;
                    audioManager.Playback.AudioStopped += OnMediaAudioStopped;

                    // 创建 VisualBrush
                    var visualBrush = new VisualBrush
                    {
                        Visual = mediaElement,
                        Stretch = Stretch.Uniform
                    };

                    // 设置 Border 背景
                    border.Background = visualBrush;
                    if (border.Child is AsyncImage image)
                    {
                        border.ContextMenu = image.ContextMenu;
                        image.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    ClearBackground();
                }
            });
        }
    }

    private static void OnMediaAudioPlaying(object? sender, Core.Events.AudioPlayingArgs e)
    {
        _media?.Play();
    }

    private static void OnMediaAudioPaused(object? sender, EventArgs e)
    {
        _media?.Pause();
    }

    private static void OnMediaAudioStopped(object? sender, EventArgs e)
    {
        ClearBackground();
    }

    private static void ClearBackground()
    {
        var audioManager = AppCore.GetRequiredService<IAudioManager>();

        audioManager.Playback.AudioPaused -= OnMediaAudioPaused;
        audioManager.Playback.AudioPlaying -= OnMediaAudioPlaying;
        audioManager.Playback.AudioStopped -= OnMediaAudioStopped;

        // 清除背景并显示 Image 控件
        _thisBorder.Background = null;
        _media = null;
        if (_thisBorder.Child is AsyncImage image)
        {
            _thisBorder.ContextMenu = null;
            image.Visibility = Visibility.Visible;
        }
    }
}