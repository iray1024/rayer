using Rayer.Core.Framework.Injection;
using Rayer.Core.Utils;
using Rayer.SearchEngine.Core.Enums;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Rayer.SearchEngine.Controls.Explore;

[Inject]
public partial class ExploreLibraryDetailPanel : UserControl
{
    private readonly Storyboard _titlebarControlStoryboard = new();
    private readonly ConcurrentDictionary<Type, RoutedEventHandler> _singletoneSubscribeHandlers = [];

    public ExploreLibraryDetailPanel()
    {
        InitializeComponent();
    }

    public static readonly RoutedEvent Checked = EventManager.RegisterRoutedEvent(
        "Checked",
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(ExploreLibraryDetailPanel));

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
        if (sender is RadioButton { IsChecked: true } radio)
        {
            var agrs = new RoutedEventArgs(Checked, e.Source);

            RaiseEvent(agrs);

            if (radio.Content is TextBlock textBlock)
            {
                var searchType = EnumHelper.ParseEnum<SearchType>(textBlock.Text);

                if (searchType is SearchType.Album)
                {
                    AlbumPanel.Visibility = Visibility.Visible;
                    PlaylistPanel.Visibility = Visibility.Collapsed;
                }                
                else
                {
                    AlbumPanel.Visibility = Visibility.Collapsed;
                    PlaylistPanel.Visibility = Visibility.Visible;
                }
            }
        }
    }

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
}