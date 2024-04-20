using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.ViewModels.Explore;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Rayer.SearchEngine.Controls;

[Inject]
public partial class SearchTitleBar : UserControl
{
    private readonly Storyboard _titlebarControlStoryboard = new();

    public SearchTitleBar()
    {
        var vm = AppCore.GetRequiredService<SearchTitleBarViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public SearchTitleBarViewModel ViewModel { get; set; }

    public static readonly RoutedEvent Checked = EventManager.RegisterRoutedEvent(
        "Checked",
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(SearchTitleBar));

    public event RoutedEventHandler CheckedChanged
    {
        add { TryAddHandler(value); }
        remove { TryRemoveHandler(value); }
    }

    private async void OnTitleBarControlMouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is RadioButton { IsChecked: true })
        {
            return;
        }

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _titlebarControlStoryboard.Stop();
            _titlebarControlStoryboard.Children.Clear();

            var animation = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            if (sender is RadioButton radio)
            {
                var innerBorder = radio.Template.FindName("CheckBorder", radio);

                Storyboard.SetTarget(animation, innerBorder as DependencyObject);
                Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));

                _titlebarControlStoryboard.Children.Add(animation);

                Timeline.SetDesiredFrameRate(_titlebarControlStoryboard, 60);

                _titlebarControlStoryboard.Begin();
            }
        });
    }

    private async void OnTitleBarControlMouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is RadioButton { IsChecked: true })
        {
            return;
        }

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _titlebarControlStoryboard.Stop();
            _titlebarControlStoryboard.Children.Clear();

            var animation = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            if (sender is RadioButton radio)
            {
                var innerBorder = radio.Template.FindName("CheckBorder", radio);

                Storyboard.SetTarget(animation, innerBorder as DependencyObject);
                Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));

                _titlebarControlStoryboard.Children.Add(animation);

                Timeline.SetDesiredFrameRate(_titlebarControlStoryboard, 60);

                _titlebarControlStoryboard.Begin();
            }
        });
    }

    private void OnChecked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton { IsChecked: true })
        {
            var agrs = new RoutedEventArgs(Checked, e.Source);

            RaiseEvent(agrs);
        }
    }

    private readonly ConcurrentDictionary<Type, RoutedEventHandler> _singletoneSubscribeHandlers = [];

    private void TryAddHandler(RoutedEventHandler handler)
    {
        if (handler.Target is not null)
        {
            if (_singletoneSubscribeHandlers.TryAdd(handler.Target.GetType(), handler))
            {
                AddHandler(Checked, handler);
            }
        }
    }

    private void TryRemoveHandler(RoutedEventHandler handler)
    {
        if (handler.Target is not null)
        {
            if (_singletoneSubscribeHandlers.TryRemove(handler.Target.GetType(), out _))
            {
                RemoveHandler(Checked, handler);
            }
        }
    }

    private async void OnSearcherSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await ViewModel.OnSearcherChanged();
    }
}