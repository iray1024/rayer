using Rayer.Core.Abstractions;
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

    public AudioFrame()
    {
        DataContext = this;

        InitializeComponent();

        audioManager = AppCore.GetRequiredService<IAudioManager>();
        audioManager.AudioChanged += OnAudioChanged;

        UpdateAudioMetadata(audioManager.Playback.Audio);
    }

    private void OnAudioChanged(object? sender, Core.Events.AudioChangedArgs e)
    {
        UpdateAudioMetadata(e.New);
    }

    public ImageSource Album
    {
        get => (ImageSource)GetValue(AlbumProperty);
        set => SetValue(AlbumProperty, value);
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

            return;
        }

        Album = audio.Cover ?? StaticThemeResources.AlbumFallback;
        Title = audio.Title;
        Information = $"{string.Join('&', audio.Artists)} - {audio.Album}";
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
}