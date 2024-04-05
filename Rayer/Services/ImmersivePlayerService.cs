using Rayer.Abstractions;
using Rayer.Controls.Immersive;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Animation;

namespace Rayer.Services;

internal partial class ImmersivePlayerService : IImmersivePlayerService
{
    private ImmersivePlayer _player = default!;
    private readonly IImmersivePresenterProvider _presenterProvider;

    private int _isNowImmersiveFlag = 0;

    public ImmersivePlayerService(IImmersivePresenterProvider presenterProvider)
    {
        _presenterProvider = presenterProvider;
    }
    
    public ImmersivePlayer Player => _player;

    public bool IsNowImmersive
    {
        get => _isNowImmersiveFlag != 0;
        set
        {
            _ = Interlocked.Exchange(ref _isNowImmersiveFlag, value ? 1 : 0);
        }
    }

    public event EventHandler? Show;
    public event EventHandler? Hidden;

    public void SetPlayer(ImmersivePlayer player)
    {
        _player = player ?? throw new ArgumentException("请正确设置ImmersivePlayer", nameof(player));
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

            _player.Presenter.Children.Add(presenter);

            if (presenter is ImmersiveVisualizerPresenter visualizerPresenter)
            {
                visualizerPresenter.Width = mainWnd.Width;
                visualizerPresenter.Height = mainWnd.Height;

                visualizerPresenter.SampleDrawingPanel.Width = mainWnd.Width + 400;
                visualizerPresenter.SampleDrawingPanel.Height = mainWnd.Height + 300;

                visualizerPresenter.Reset_Visualizer(256);
                visualizerPresenter.AudioVisualizerStoryboard.Begin();
            }

            _player.Visibility = Visibility.Visible;

            await PlayerFadeInOutAsync();

            _player.GridBottomBlurEffect.Radius = 160;
        }
        else
        {
            App.MainWindow.SizeChanged -= OnSizeChanged;

            Hidden?.Invoke(null, EventArgs.Empty);

            actualWnd.TitleBar.Visibility = Visibility.Visible;
            actualWnd.TitleBar.ShowMinimize = true;
            actualWnd.TitleBar.ShowMaximize = true;
            actualWnd.TitleBar.ShowClose = true;

            await PlayerFadeInOutAsync(false);
        }
    }

    private async Task PlayerFadeInOutAsync(bool fadeIn = true)
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

            Storyboard.SetTarget(animation, _player);
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
        _player.Visibility ^= Visibility.Hidden;

        if (_player.Presenter.Children[0] is ImmersiveVisualizerPresenter visualizerPresenter)
        {
            ResetAudioVisualiazer(visualizerPresenter);
        }

        _player.Presenter.Children.Clear();
    }

    internal void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_player.Presenter.Children[0] is FrameworkElement element)
        {
            element.Width = e.NewSize.Width;
            element.Height = e.NewSize.Height;
        }
    }

    private void ResetAudioVisualiazer(ImmersiveVisualizerPresenter visualizerPresenter)
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
                    visualizerPresenter.spectrumData = null;
                }

                if (visualizerPresenter.pathGeometries is not null)
                {
                    visualizerPresenter.pathGeometries.Clear();
                    visualizerPresenter.pathGeometries = null;
                }

                visualizerPresenter.AudioVisualizerStoryboard.Stop();

                visualizerPresenter.Close_Visualizer();

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);
}