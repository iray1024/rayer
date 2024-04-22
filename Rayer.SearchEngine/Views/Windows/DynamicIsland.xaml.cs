using Microsoft.Extensions.Options;
using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Http;
using Rayer.Core.PInvoke;
using Rayer.SearchEngine.Core.Options;
using Rayer.SearchEngine.ViewModels;
using System.Net.Http;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Wpf.Ui.Appearance;

namespace Rayer.SearchEngine.Views.Windows;

[Inject]
public partial class DynamicIsland : Window
{
    private readonly Storyboard? _dynamicIslandStoryboard = new();
    private static readonly Storyboard _windowStateChangedStoryboard = new();
    private string _currentScrrenDeviceName = string.Empty;
    private static WindowState _lastWindowState = WindowState.Normal;

    public DynamicIsland()
    {
        var vm = AppCore.GetRequiredService<DynamicIslandViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();

        TextBlurStroyboard = Resources["TextBlurStroyboard"] as Storyboard
            ?? throw new ArgumentNullException("未找到 AlbumRotateStoryboard 资源");

        _windowStateChangedStoryboard.Completed += static (s, e) =>
        {
            _windowStateChangedStoryboard.Stop();
        };
    }

    public DynamicIslandViewModel ViewModel { get; set; }

    public Storyboard TextBlurStroyboard { get; }

    private async void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            if (_dynamicIslandStoryboard is not null)
            {
                var currentPanelWitdh = Panel.ActualWidth;
                var currentWith = Lyric.ActualWidth;

                _dynamicIslandStoryboard.Stop();
                _dynamicIslandStoryboard.Children.Clear();

                var restoreWidth = currentWith - 100 < 80 ? 120 : currentWith - 100;
                var stretchWidth = currentWith + 111;
                var finalWidth = currentWith + 71;

                var restoreAnimation = new DoubleAnimation
                {
                    From = currentPanelWitdh,
                    To = restoreWidth,
                    Duration = TimeSpan.FromMilliseconds(250),
                    EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut },
                    BeginTime = TimeSpan.Zero
                };

                var stretchAnimation = new DoubleAnimation
                {
                    From = restoreWidth,
                    To = stretchWidth,
                    Duration = TimeSpan.FromMilliseconds(250),
                    EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut },
                    BeginTime = new TimeSpan(0, 0, 0, 0, 250)
                };

                var shortenAnimation = new DoubleAnimation
                {
                    From = stretchWidth,
                    To = finalWidth,
                    Duration = TimeSpan.FromMilliseconds(600),
                    EasingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut },
                    BeginTime = new TimeSpan(0, 0, 0, 0, 500)
                };

                Storyboard.SetTarget(restoreAnimation, Panel);
                Storyboard.SetTargetProperty(restoreAnimation, new PropertyPath(WidthProperty));
                Storyboard.SetTarget(stretchAnimation, Panel);
                Storyboard.SetTargetProperty(stretchAnimation, new PropertyPath(WidthProperty));
                Storyboard.SetTarget(shortenAnimation, Panel);
                Storyboard.SetTargetProperty(shortenAnimation, new PropertyPath(WidthProperty));

                _dynamicIslandStoryboard.Children.Add(restoreAnimation);
                _dynamicIslandStoryboard.Children.Add(stretchAnimation);
                _dynamicIslandStoryboard.Children.Add(shortenAnimation);

                Timeline.SetDesiredFrameRate(_dynamicIslandStoryboard, 60);

                _dynamicIslandStoryboard.Begin();
            }
        });
    }

    private void OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void SetInitializePosition()
    {
        var wnd = Application.Current.MainWindow;

        var mainScreen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(wnd).Handle);

        _currentScrrenDeviceName = mainScreen.DeviceName;

        if (mainScreen is not null)
        {
            var currenScreenTop = mainScreen.Bounds.Top + 50;

            Top = currenScreenTop;
        }
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        SetInitializePosition();

        SystemThemeWatcher.UnWatch(this);

        ViewModel.DynamicIsland = this;

        Application.Current.MainWindow.LocationChanged += OnLocationChanged;
        Application.Current.MainWindow.StateChanged += OnStateChanged;

        var hwnd = new WindowInteropHelper(this).Handle;

        var exStyle = Win32.User32.GetWindowLong(hwnd, Win32.User32.GWL_EXSTYLE);

        _ = Win32.User32.SetWindowLong(hwnd, Win32.User32.GWL_EXSTYLE, exStyle | Win32.User32.WS_EX_TOOLWINDOW);

        var searchEngineOptions = AppCore.GetRequiredService<IOptionsSnapshot<SearchEngineOptions>>().Value;
        var httpClient = AppCore.GetRequiredService<IHttpClientProvider>();

        try
        {
            await httpClient.HttpClient.GetAsync($"{searchEngineOptions.HttpEndpoint}");
        }
        catch (HttpRequestException)
        {
            searchEngineOptions.HttpEndpoint = "https://netease-cloud-music-api-rayer.vercel.app";
        }
    }

    private void OnLocationChanged(object? sender, EventArgs e)
    {
        var currentScreen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(Application.Current.MainWindow).Handle);

        if (currentScreen.DeviceName != _currentScrrenDeviceName)
        {
            _windowStateChangedStoryboard.Stop();
            _currentScrrenDeviceName = currentScreen.DeviceName;

            Left = ((currentScreen.Bounds.Width - ActualWidth) / 2) + currentScreen.Bounds.Left;
            Top = currentScreen.Bounds.Top + 50;
        }
    }

    private void OnStateChanged(object? sender, EventArgs e)
    {
        if (AppCore.MainWindow.WindowState is WindowState.Maximized && _lastWindowState != WindowState.Minimized)
        {
            var targetTop = Top + 20;

            _windowStateChangedStoryboard.Stop();
            _windowStateChangedStoryboard.Children.Clear();

            var animation = new DoubleAnimation()
            {
                From = Top,
                To = targetTop,
                Duration = TimeSpan.FromMilliseconds(800),
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath(TopProperty));

            _windowStateChangedStoryboard.Children.Add(animation);

            Timeline.SetDesiredFrameRate(_windowStateChangedStoryboard, 120);

            _windowStateChangedStoryboard.Begin();
        }
        else if (AppCore.MainWindow.WindowState is WindowState.Normal && _lastWindowState != WindowState.Minimized)
        {
            var targetTop = Top - 20;

            _windowStateChangedStoryboard.Stop();
            _windowStateChangedStoryboard.Children.Clear();

            var animation = new DoubleAnimation()
            {
                From = Top,
                To = targetTop,
                Duration = TimeSpan.FromMilliseconds(800),
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath(TopProperty));

            _windowStateChangedStoryboard.Children.Add(animation);

            Timeline.SetDesiredFrameRate(_windowStateChangedStoryboard, 120);

            _windowStateChangedStoryboard.Begin();
        }

        _lastWindowState = AppCore.MainWindow.WindowState;
    }
}