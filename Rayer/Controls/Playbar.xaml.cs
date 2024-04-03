using NAudio.Wave;
using Rayer.Controls.Adorners;
using Rayer.Core.Abstractions;
using Rayer.Core.Models;
using Rayer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace Rayer.Controls;

public partial class Playbar : UserControl
{
    private static PlaybarResource _resource;

    private readonly IThemeResourceProvider _themeResourceProvider;
    private readonly IPlaybarService _playbarService;

    private static Rect _bounds;

    private bool _isAdornerVisible = false;

    public Playbar()
    {
        var vm = App.GetRequiredService<PlaybarViewModel>();

        ViewModel = vm;
        DataContext = this;

        _playbarService = App.GetRequiredService<IPlaybarService>();
        _themeResourceProvider = App.GetRequiredService<IThemeResourceProvider>();
        var audioManager = App.GetRequiredService<IAudioManager>();

        ApplicationThemeManager.Changed += ThemeChanged;

        _playbarService.PlayOrPauseTriggered += OnPlayOrPauseTriggered;
        audioManager.AudioStopped += OnAudioStopped;

        InitializeResource();

        InitializeComponent();

        _bounds = new Rect(0, 0, ActualWidth, ActualHeight);
    }

    private void OnAudioStopped(object? sender, EventArgs e)
    {
        ToggleControlsState(false);

        PlayOrPause.Source = _resource.Play;
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

    private void GlobalStop()
    {
        ViewModel.AudioManager.Playback.Stop();

        ToggleControlsState(false);

        PlayOrPause.Source = _resource.Play;
    }

    #region Slider Events
    private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
    {
        ViewModel.IgnoreUpdateProgressValue = true;
    }

    private void Slider_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
        ViewModel.AudioManager.Playback.Seek(ViewModel.ProgressValue);
    }

    private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
    {
        ViewModel.AudioManager.Playback.Seek(ViewModel.ProgressValue);

        ViewModel.IgnoreUpdateProgressValue = false;
    }

    private void Slider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (ViewModel.AudioManager.Playback.PlaybackState is PlaybackState.Paused)
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

    private void ThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
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

        var element = (UIElement)Controller;

        var adornerLayer = AdornerLayer.GetAdornerLayer(element);
        var adnoners = adornerLayer.GetAdorners(element);

        if (adornerLayer is not null && (adnoners is null || adnoners.Length == 0))
        {
            adornerLayer.Add(new PlayloopAdorner(element));
            adornerLayer.Add(new VolumeAdorner(element));
            adornerLayer.Add(new PitchAdorner(element));
        }
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        if (_bounds.Contains(e.GetPosition(this)))
        {
            return;
        }

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

        _isAdornerVisible = false;
    }
}