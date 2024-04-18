using Rayer.Core.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Rayer.SearchEngine.Controls.Explore;

public partial class ExploreLibraryPainedAudioPanel : UserControl
{
    private readonly Storyboard _hoverableControlStoryboard = new();

    public ExploreLibraryPainedAudioPanel()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.RegisterAttached(
                "IsChecked",
                typeof(bool),
                typeof(ExploreLibraryPainedAudioPanel),
                new PropertyMetadata(false)
            );

    public static bool GetIsChecked(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsCheckedProperty);
    }

    public static void SetIsChecked(DependencyObject obj, bool value)
    {
        obj.SetValue(IsCheckedProperty, value);
    }

    private async void OnMouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Border border && GetIsChecked(border))
        {
            return;
        }

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _hoverableControlStoryboard.Stop();
            _hoverableControlStoryboard.Children.Clear();

            var animation = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            if (sender is Border border)
            {
                Storyboard.SetTarget(animation, border);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Background).(Brush.Opacity)"));

                _hoverableControlStoryboard.Children.Add(animation);

                Timeline.SetDesiredFrameRate(_hoverableControlStoryboard, 60);

                _hoverableControlStoryboard.Begin();
            }
        });
    }

    private async void OnMouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Border border && GetIsChecked(border))
        {
            return;
        }

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _hoverableControlStoryboard.Stop();
            _hoverableControlStoryboard.Children.Clear();

            var animation = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            if (sender is Border border)
            {
                Storyboard.SetTarget(animation, border);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Background).(Brush.Opacity)"));

                _hoverableControlStoryboard.Children.Add(animation);

                Timeline.SetDesiredFrameRate(_hoverableControlStoryboard, 60);

                _hoverableControlStoryboard.Begin();
            }
        });
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2 && sender is Border border)
        {
            var currentThemeBrush = (SolidColorBrush)Application.Current.Resources["ControlStrokeColorDefaultBrush"];
            var currentTextPrimaryBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            var currentTextSecondaryBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorSecondaryBrush"];

            foreach (var item in ItemGroup.Items)
            {
                var vItem = ItemGroup.ItemContainerGenerator.ContainerFromItem(item);

                var presenter = ElementHelper.FindVisualChild<Border>(vItem);

                var vBorder = presenter.FindName("PART_Border") as Border;

                if (vBorder is not null)
                {
                    vBorder.Background = new SolidColorBrush(currentThemeBrush.Color)
                    {
                        Opacity = 0
                    };

                    if (vBorder.Child is Grid vInnerGrid)
                    {
                        ((Wpf.Ui.Controls.TextBlock)vInnerGrid.Children[1]).Foreground = new SolidColorBrush(currentTextPrimaryBrush.Color);
                        ((Wpf.Ui.Controls.TextBlock)vInnerGrid.Children[2]).Foreground = new SolidColorBrush(currentTextSecondaryBrush.Color);
                    }

                    SetIsChecked(vBorder, false);
                }
            }

            border.Background = new SolidColorBrush(Color.FromRgb(187, 205, 255));

            if (border.Child is Grid innerGrid)
            {
                ((Wpf.Ui.Controls.TextBlock)innerGrid.Children[1]).Foreground = new SolidColorBrush(Color.FromRgb(51, 94, 234));
                ((Wpf.Ui.Controls.TextBlock)innerGrid.Children[2]).Foreground = new SolidColorBrush(Color.FromRgb(51, 94, 234));
            }

            SetIsChecked(border, true);
        }
    }
}