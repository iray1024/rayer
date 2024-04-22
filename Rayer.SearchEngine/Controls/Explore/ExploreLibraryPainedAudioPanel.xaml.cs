using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Events;
using Rayer.Core.Models;
using Rayer.Core.Utils;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Core.Domain.Aduio;
using Rayer.SearchEngine.Internal.Effects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Rayer.SearchEngine.Controls.Explore;

public partial class ExploreLibraryPainedAudioPanel : UserControl
{
    private readonly IAudioManager _audioManager;

    private readonly Storyboard _hoverableControlStoryboard = new();

    public ExploreLibraryPainedAudioPanel()
    {
        InitializeComponent();

        _audioManager = AppCore.GetRequiredService<IAudioManager>();

        _audioManager.AudioChanged += OnAudioChanged;
        _audioManager.AudioStopped += OnAudioStopped;
    }

    public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.RegisterAttached(
                "IsChecked",
                typeof(bool),
                typeof(ExploreLibraryPainedAudioPanel),
                new PropertyMetadata(false)
            );

    public static readonly DependencyProperty IsPlayableProperty =
            DependencyProperty.RegisterAttached(
                "IsPlayable",
                typeof(bool),
                typeof(ExploreLibraryPainedAudioPanel),
                new PropertyMetadata(true)
            );

    public static bool GetIsChecked(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsCheckedProperty);
    }

    public static void SetIsChecked(DependencyObject obj, bool value)
    {
        obj.SetValue(IsCheckedProperty, value);
    }

    public static bool GetIsPlayable(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsPlayableProperty);
    }

    public static void SetIsPlayable(DependencyObject obj, bool value)
    {
        obj.SetValue(IsPlayableProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {

    }

    private void OnUnLoaded(object sender, RoutedEventArgs e)
    {

    }

    private async void OnMouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Border border && (GetIsChecked(border) || !GetIsPlayable(border)))
        {
            return;
        }

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
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
        if (sender is Border border && (GetIsChecked(border) || !GetIsPlayable(border)))
        {
            return;
        }

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
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

    private async void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2 && sender is Border border &&
            border.DataContext is SearchAudioDetail detail)
        {
            if (!GetIsPlayable(border))
            {
                return;
            }

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
                    if (GetIsPlayable(vBorder))
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
            }

            if (GetIsPlayable(border))
            {
                border.Background = new SolidColorBrush(Color.FromRgb(187, 205, 255));

                if (border.Child is Grid innerGrid)
                {
                    ((Wpf.Ui.Controls.TextBlock)innerGrid.Children[1]).Foreground = new SolidColorBrush(Color.FromRgb(51, 94, 234));
                    ((Wpf.Ui.Controls.TextBlock)innerGrid.Children[2]).Foreground = new SolidColorBrush(Color.FromRgb(51, 94, 234));
                }

                SetIsChecked(border, true);

                await Play(detail);
            }
        }
    }

    private void OnAudioChanged(object? sender, AudioChangedArgs e)
    {
        var currentThemeBrush = (SolidColorBrush)Application.Current.Resources["ControlStrokeColorDefaultBrush"];
        var currentTextPrimaryBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
        var currentTextSecondaryBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorSecondaryBrush"];

        foreach (var item in ItemGroup.Items)
        {
            var vItem = ItemGroup.ItemContainerGenerator.ContainerFromItem(item);

            var presenter = ElementHelper.FindVisualChild<Border>(vItem);

            var vBorder = presenter.FindName("PART_Border") as Border;

            if (vBorder is not null &&
                vBorder.DataContext is SearchAudioDetail detail)
            {
                if (GetIsChecked(vBorder) && GetIsPlayable(vBorder) && detail.Id != e.New.Id)
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
                else if (!GetIsChecked(vBorder) && GetIsPlayable(vBorder) && detail.Id == e.New.Id)
                {
                    vBorder.Background = new SolidColorBrush(Color.FromRgb(187, 205, 255));

                    if (vBorder.Child is Grid vInnerGrid)
                    {
                        ((Wpf.Ui.Controls.TextBlock)vInnerGrid.Children[1]).Foreground = new SolidColorBrush(Color.FromRgb(51, 94, 234));
                        ((Wpf.Ui.Controls.TextBlock)vInnerGrid.Children[2]).Foreground = new SolidColorBrush(Color.FromRgb(51, 94, 234));
                    }

                    SetIsChecked(vBorder, true);
                }
            }
        }
    }

    private void OnAudioStopped(object? sender, EventArgs e)
    {
        var currentThemeBrush = (SolidColorBrush)Application.Current.Resources["ControlStrokeColorDefaultBrush"];
        var currentTextPrimaryBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
        var currentTextSecondaryBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorSecondaryBrush"];

        foreach (var item in ItemGroup.Items)
        {
            var vItem = ItemGroup.ItemContainerGenerator.ContainerFromItem(item);

            var presenter = ElementHelper.FindVisualChild<Border>(vItem);

            var vBorder = presenter.FindName("PART_Border") as Border;

            if (vBorder is not null && GetIsChecked(vBorder))
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
    }

    private static async Task Play(SearchAudioDetail detail)
    {
        var provider = AppCore.GetRequiredService<ISearchAudioEngineProvider>();
        var audioManager = AppCore.GetRequiredService<IAudioManager>();

        var audioInformation = await provider.GetAudioEngine(SearcherType.Netease).GetAudioAsync(detail);

        if (!audioManager.Playback.TryGetAudio(detail.Id, out var existsAudio))
        {
            var audio = new Audio()
            {
                Id = detail.Id,
                Title = detail.Title,
                Artists = detail.Artists.Select(x => x.Name).ToArray(),
                Album = detail.Album?.Title ?? string.Empty,
                Cover = detail.Album?.Picture is not null ? ImageSourceUtils.Create(detail.Album.Picture) : null,
                Duration = detail.Duration,
                Path = audioInformation.Url ?? string.Empty,
                IsVirualWebSource = true,
                SearcherType = SearcherType.Netease
            };

            audioManager.Playback.Queue.Add(audio);

            await audioManager.Playback.Play(audio);
        }
        else
        {
            await audioManager.Playback.Play(existsAudio);
        }
    }

    private void OnItemDataLoaded(object sender, RoutedEventArgs e)
    {
        RefreshState();
    }

    private void RefreshState()
    {
        foreach (var item in ItemGroup.Items)
        {
            var vItem = ItemGroup.ItemContainerGenerator.ContainerFromItem(item);

            var presenter = ElementHelper.FindVisualChild<Border>(vItem);

            var vBorder = presenter.FindName("PART_Border") as Border;

            if (vBorder is not null)
            {
                if (vBorder.DataContext is SearchAudioDetail detail)
                {
                    if (!string.IsNullOrEmpty(detail.Copyright.Reason))
                    {
                        SetIsPlayable(vBorder, false);
                        vBorder.ToolTip = detail.Copyright.Reason;

                        if (vBorder.Child is Grid innerGrid)
                        {
                            ((Image)innerGrid.Children[0]).Effect = new GrayscaleBitmapEffect();
                            ((Wpf.Ui.Controls.TextBlock)innerGrid.Children[1]).Foreground = new SolidColorBrush(Color.FromRgb(96, 96, 96));
                            ((Wpf.Ui.Controls.TextBlock)innerGrid.Children[2]).Foreground = new SolidColorBrush(Color.FromRgb(96, 96, 96));
                        }
                    }

                    if (_audioManager.Playback.Audio is not null && detail.Id == _audioManager.Playback.Audio.Id && GetIsPlayable(vBorder))
                    {
                        vBorder.Background = new SolidColorBrush(Color.FromRgb(187, 205, 255));

                        if (vBorder.Child is Grid innerGrid)
                        {
                            ((Wpf.Ui.Controls.TextBlock)innerGrid.Children[1]).Foreground = new SolidColorBrush(Color.FromRgb(51, 94, 234));
                            ((Wpf.Ui.Controls.TextBlock)innerGrid.Children[2]).Foreground = new SolidColorBrush(Color.FromRgb(51, 94, 234));
                        }

                        SetIsChecked(vBorder, true);
                    }
                }
            }
        }
    }
}