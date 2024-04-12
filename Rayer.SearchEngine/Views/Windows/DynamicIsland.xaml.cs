using Rayer.Core;
using Rayer.SearchEngine.ViewModels;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Wpf.Ui.Appearance;

namespace Rayer.SearchEngine.Views.Windows;

public partial class DynamicIsland : Window
{
    private readonly Storyboard? _dynamicIslandStoryboard = new();

    public DynamicIsland()
    {
        var vm = AppCore.GetRequiredService<DynamicIslandViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();

        TextBlurStroyboard = Resources["TextBlurStroyboard"] as Storyboard
            ?? throw new ArgumentNullException("未找到 AlbumRotateStoryboard 资源");        
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

        if (mainScreen is not null)
        {
            var currenScreenTop = mainScreen.Bounds.Top + 50;

            Top = currenScreenTop;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        SetInitializePosition();

        SystemThemeWatcher.UnWatch(this);

        ViewModel.DynamicIsland = this;
    }
}