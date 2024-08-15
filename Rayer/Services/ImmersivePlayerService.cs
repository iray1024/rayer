using Rayer.Abstractions;
using Rayer.Controls.Immersive;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using System.Windows;
using System.Windows.Media.Animation;

namespace Rayer.Services;

[Inject<IImmersivePlayerService>]
internal partial class ImmersivePlayerService : IImmersivePlayerService
{
    private readonly IImmersivePresenterProvider _presenterProvider;
    private readonly IAudioManager _audioManager;

    private int _isNowImmersiveFlag = 0;
    private static readonly int _immersiveModeMaxValue = (int)Enum.GetValues<ImmersiveMode>().Max();

    public ImmersivePlayerService(
        IAudioManager audioManager,
        IImmersivePresenterProvider presenterProvider)
    {
        _audioManager = audioManager;
        _presenterProvider = presenterProvider;
    }

    public ImmersivePlayer Player { get; private set; } = default!;

    public bool IsNowImmersive
    {
        get => _isNowImmersiveFlag != 0;
        set => _ = Interlocked.Exchange(ref _isNowImmersiveFlag, value ? 1 : 0);
    }

    public event EventHandler? Show;
    public event EventHandler? Hidden;

    public void SetPlayer(ImmersivePlayer player)
    {
        Player = player ?? throw new ArgumentException("请正确设置ImmersivePlayer", nameof(player));
    }

    public async Task ToggleShow()
    {
        IsNowImmersive = !IsNowImmersive;

        var mainWnd = Application.Current.MainWindow;

        var actualWnd = App.MainWindow;

        if (IsNowImmersive)
        {
            App.MainWindow.SizeChanged += OnSizeChanged;

            Show?.Invoke(null, EventArgs.Empty);

            actualWnd.TitleBar.Visibility = Visibility.Collapsed;
            actualWnd.TitleBar.ShowMinimize = false;
            actualWnd.TitleBar.ShowMaximize = false;
            actualWnd.TitleBar.ShowClose = false;

            var presenter = _presenterProvider.Presenter;

            Player.Presenter.Children.Add(presenter);

            if (presenter is ImmersiveVisualizerPresenter visualizerPresenter)
            {
                visualizerPresenter.Width = mainWnd.Width;
                visualizerPresenter.Height = mainWnd.Height;

                visualizerPresenter.SampleDrawingPanel.Width = mainWnd.Width * 2;
                visualizerPresenter.SampleDrawingPanel.Height = mainWnd.Height * 2;

                visualizerPresenter.Reset_Visualizer(256);
                if (_audioManager.Playback.DeviceManager.PlaybackState is NAudio.Wave.PlaybackState.Playing)
                {
                    visualizerPresenter.AudioVisualizerStoryboard.Begin();
                }
            }
            else if (presenter is ImmersiveVinylPresenter vinylPresenter)
            {
                if (_audioManager.Playback.DeviceManager.PlaybackState is NAudio.Wave.PlaybackState.Playing)
                {
                    vinylPresenter.AlbumRotateStoryboard.Begin();
                }
            }

            Player.Visibility = Visibility.Visible;

            await PlayerFadeInOutAsync(Player);

            Player.GridBottomBlurEffect.Radius = 160;
        }
        else
        {
            App.MainWindow.SizeChanged -= OnSizeChanged;

            Hidden?.Invoke(null, EventArgs.Empty);

            actualWnd.TitleBar.Visibility = Visibility.Visible;
            actualWnd.TitleBar.ShowMinimize = true;
            actualWnd.TitleBar.ShowMaximize = true;
            actualWnd.TitleBar.ShowClose = true;

            await PlayerFadeInOutAsync(Player, false);
        }
    }

    public async Task Switch()
    {
        var settingsService = App.GetRequiredService<ISettingsService>();

        settingsService.Settings.ImmersiveMode = (ImmersiveMode)(((int)settingsService.Settings.ImmersiveMode + 1) % (_immersiveModeMaxValue + 1));
        settingsService.Save();

        var presenter = _presenterProvider.Presenter;

        var previousPresenter = Player.Presenter.Children[0];

        Player.Presenter.Children.Remove(previousPresenter);
        Player.Presenter.Children.Add(presenter);

        await PlayerFadeInOutAsync(Player.Presenter, true);

        if (presenter is ImmersiveVisualizerPresenter visualizerPresenter)
        {
            var mainWnd = Application.Current.MainWindow;

            visualizerPresenter.Width = mainWnd.ActualWidth;
            visualizerPresenter.Height = mainWnd.ActualHeight;

            visualizerPresenter.SampleDrawingPanel.Width = mainWnd.Width * 2;
            visualizerPresenter.SampleDrawingPanel.Height = mainWnd.Height * 2;

            visualizerPresenter.Reset_Visualizer(256);
            visualizerPresenter.AudioVisualizerStoryboard.Begin();
        }
        else
        {
            if (previousPresenter is ImmersiveVisualizerPresenter _visualizerPresenter)
            {
                ResetAudioVisualiazer(_visualizerPresenter);
            }

            if (presenter is ImmersiveVinylPresenter vinylPresenter)
            {
                vinylPresenter.AlbumRotateStoryboard.Begin();
            }
        }
    }

    private async Task PlayerFadeInOutAsync(DependencyObject target, bool fadeIn = true)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var animation = new DoubleAnimation
            {
                From = fadeIn ? 0 : 1,
                To = fadeIn ? 1 : 0,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new CubicEase()
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));

            if (!fadeIn)
            {
                storyboard.Completed += OnStoryboardCompleted;
            }

            Timeline.SetDesiredFrameRate(storyboard, 60);
            storyboard.Begin();
        });
    }

    private void OnStoryboardCompleted(object? sender, EventArgs e)
    {
        Player.Visibility ^= Visibility.Hidden;

        if (Player.Presenter.Children[0] is ImmersiveVisualizerPresenter visualizerPresenter)
        {
            ResetAudioVisualiazer(visualizerPresenter);
        }

        Player.Presenter.Children.Clear();
    }

    internal void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (Player.Presenter.Children[0] is ImmersiveVisualizerPresenter element)
        {
            element.Width = e.NewSize.Width;
            element.Height = e.NewSize.Height;
        }
    }

    private static void ResetAudioVisualiazer(ImmersiveVisualizerPresenter visualizerPresenter)
    {
        if (visualizerPresenter.capture is not null)
        {
            try
            {
                if (visualizerPresenter.dataTimer is not null)
                {
                    visualizerPresenter.dataTimer.Dispose();
                    visualizerPresenter.dataTimer = null;
                }

                if (visualizerPresenter.drawingTimer is not null)
                {
                    visualizerPresenter.drawingTimer.Dispose();
                    visualizerPresenter.drawingTimer = null;
                }

                if (visualizerPresenter.spectrumData is not null)
                {
                    visualizerPresenter.spectrumData.Clear();
                    visualizerPresenter.spectrumData = default!;
                }

                if (visualizerPresenter.pathGeometries is not null)
                {
                    visualizerPresenter.pathGeometries.Clear();
                    visualizerPresenter.pathGeometries = default!;
                }

                visualizerPresenter.AudioVisualizerStoryboard.Stop();

                visualizerPresenter.Close_Visualizer();

            }
            catch
            {
                throw;
            }
        }
    }
}