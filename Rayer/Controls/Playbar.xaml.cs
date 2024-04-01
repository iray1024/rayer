using NAudio.Wave;
using Rayer.Core.Abstractions;
using Rayer.Core.Models;
using Rayer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Appearance;

namespace Rayer.Controls;

public partial class Playbar : UserControl
{
    private static PlaybarResource _resource;

    private readonly IThemeResourceProvider _themeResourceProvider;

    public Playbar()
    {
        var vm = App.GetRequiredService<PlaybarViewModel>();

        ViewModel = vm;
        DataContext = this;

        _themeResourceProvider = App.GetRequiredService<IThemeResourceProvider>();

        ApplicationThemeManager.Changed += ThemeChanged;

        InitializeResource();

        InitializeComponent();
    }

    public PlaybarViewModel ViewModel { get; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
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
        }
    }

    private void PlayOrPauseClick(object sender, MouseButtonEventArgs e)
    {
        ViewModel.PlayOrPause();

        SetPlayOrPauseTheme();
    }

    private void OnPlaybarPlaying(object? sender, EventArgs e)
    {
        ToggleControlsState(true);

        PlayOrPause.Source = _resource.Pause;
    }

    private async void PreviousClick(object sender, MouseButtonEventArgs e)
    {
        await ViewModel.AudioManager.Playback.Previous();
    }

    private async void NextClick(object sender, MouseButtonEventArgs e)
    {
        await ViewModel.AudioManager.Playback.Next();
    }

    private void GlobalStop()
    {
        ViewModel.AudioManager.Playback.Stop();

        ToggleControlsState(false);

        PlayOrPause.Source = _resource.Play;
    }

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

    private void ToggleControlsState(bool enable = true)
    {
        Previous.IsEnabled = enable;
        PlayOrPause.IsEnabled = enable;
        Next.IsEnabled = enable;
        PlaybarSlider.IsEnabled = enable;
    }

    private void ThemeChanged(ApplicationTheme currentApplicationTheme, System.Windows.Media.Color systemAccent)
    {
        _resource = _themeResourceProvider.GetPlaybarResource();

        SetPlayOrPauseTheme();

        Previous.Source = _resource.Previous;
        Next.Source = _resource.Next;
    }

    private void SetPlayOrPauseTheme()
    {
        var state = ViewModel.AudioManager.Playback.PlaybackState;

        PlayOrPause.Source = state is PlaybackState.Playing
            ? _resource.Pause
            : _resource.Play;
    }
}