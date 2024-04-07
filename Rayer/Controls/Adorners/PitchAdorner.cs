using Rayer.Abstractions;
using Rayer.Core.Abstractions;
using Rayer.Services;
using Rayer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Rayer.Controls.Adorners;

public class PitchAdorner : Adorner
{
    private static readonly PlaybarViewModel _vm;
    private readonly IImmersivePlayerService _immersivePlayerService;

    private static PitchPanel _panel = default!;
    private static ImageIcon _internalImage = default!;
    private static Slider _internalSlider = default!;

    private static readonly float _semitone = MathF.Pow(2, 1.0f / 12);
    private static readonly float _upOneTone = _semitone * _semitone;
    private static readonly float _downOneTone = 1.0f / _upOneTone;

    static PitchAdorner()
    {
        _vm = App.GetRequiredService<PlaybarViewModel>();

        var playbarService = App.GetRequiredService<IPlaybarService>();

        playbarService.PitchUpTriggered += OnPitchUpTriggered;
        playbarService.PitchDownTriggered += OnPitchDownTriggered;
    }

    public PitchAdorner(UIElement adornedElement)
        : base(adornedElement)
    {
        _panel = new PitchPanel();

        _immersivePlayerService = App.GetRequiredService<IImmersivePlayerService>();

        _immersivePlayerService.Show += OnSwitchImmersivePlayerDisplay;
        _immersivePlayerService.Hidden += OnSwitchImmersivePlayerDisplay;

        AddVisualChild(_panel);

        SetInternalControls();

        ToolTipService.SetToolTip(_internalImage, "重置音频");
        ToolTipService.SetToolTip(_internalSlider, GetToolTip());

        ApplicationThemeManager.Changed += OnThemeChanged;
    }

    private void OnSwitchImmersivePlayerDisplay(object? sender, EventArgs e)
    {
        SetInternalImageIconTheme();
    }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        if (_panel.Content is Grid grid)
        {
            foreach (var item in grid.Children)
            {
                if (item is ImageIcon icon && icon.Name == "Pitch")
                {
                    RenderOptions.SetBitmapScalingMode(icon, BitmapScalingMode.Fant);
                    icon.Source = (ImageSource)Application.Current.Resources["Pitch"];
                }
            }
        }
    }

    protected override int VisualChildrenCount => 1;

    protected override Visual GetVisualChild(int index)
    {
        return _panel;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _panel.Arrange(new Rect(
            new Point(AdornedElement.DesiredSize.Width + _panel.ActualWidth - 82, 5),
            new Size(150, 24)));

        return finalSize;
    }

    #region Pitch    
    private void SetInternalControls()
    {
        if (_panel.Content is Grid grid)
        {
            foreach (var item in grid.Children)
            {
                if (item is ImageIcon image && image.Name == "Pitch")
                {
                    RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
                    image.Source = _immersivePlayerService.IsNowImmersive
                        ? StaticThemeResources.Dark.Pitch
                        : (ImageSource)Application.Current.Resources["Pitch"];

                    _internalImage = image;
                }
                if (item is Slider slider && slider.Name == "PitchSlider")
                {
                    slider.MouseWheel += OnPitchMouseWheel;
                    slider.ValueChanged += OnValueChanged;
                    slider.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnDragDelta), true);
                    slider.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnDragCompleted), true);

                    _internalSlider = slider;

                    if (_immersivePlayerService.IsNowImmersive)
                    {
                        _internalSlider.Loaded += OnInternalSliderLoaded;

                    }
                }
            }
        }
    }



    private void OnInternalSliderLoaded(object sender, RoutedEventArgs e)
    {
        var trackBorder = (Border)_internalSlider.Template.FindName("TrackBackground", _internalSlider);
        trackBorder.Background = StaticThemeResources.Dark.SliderTrackFill;

        _internalSlider.Loaded -= OnInternalSliderLoaded;
    }

    private void SetInternalImageIconTheme()
    {
        if (_panel.Content is Grid grid)
        {
            foreach (var item in grid.Children)
            {
                if (item is ImageIcon image && image.Name == "Pitch")
                {
                    RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);

                    if (_immersivePlayerService.IsNowImmersive)
                    {
                        image.Source = StaticThemeResources.Dark.Pitch;

                        var trackBorder = (Border)_internalSlider.Template.FindName("TrackBackground", _internalSlider);
                        trackBorder.Background = StaticThemeResources.Dark.SliderTrackFill;
                    }
                    else
                    {
                        image.Source = (ImageSource)StaticThemeResources.GetDynamicResource("Pitch");

                        var trackBorder = (Border)_internalSlider.Template.FindName("TrackBackground", _internalSlider);
                        trackBorder.SetResourceReference(Control.BackgroundProperty, "SliderTrackFill");
                    }
                }
            }
        }
    }

    private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var factor = MathF.Round((float)_internalSlider.Value, 2);

        _vm.AudioManager.Playback.Pitch = factor;

        Save(factor);
    }

    private void OnPitchMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var factor = _vm.AudioManager.Playback.Pitch + (0.05f * (e.Delta > 0 ? 1 : -1));

        factor = Math.Min(Math.Max(factor, 0.5f), 2f);

        _internalSlider.Value = factor;
        _vm.AudioManager.Playback.Pitch = factor;
        Save(factor);
    }

    private void OnDragDelta(object sender, DragDeltaEventArgs e)
    {
        var factor = (float)_internalSlider.Value;

        _vm.AudioManager.Playback.Pitch = factor;
    }

    private void OnDragCompleted(object sender, DragCompletedEventArgs e)
    {
        var factor = (float)_internalSlider.Value;

        _vm.AudioManager.Playback.Pitch = factor;

        Save(factor);
    }

    private static void Save(float factor)
    {
        ToolTipService.SetToolTip(_internalSlider, GetToolTip());

        _vm.SettingsService.Settings.Pitch = MathF.Round(factor, 2);
        _vm.SettingsService.Save();
    }
    #endregion

    private static void OnPitchUpTriggered(object? sender, EventArgs e)
    {
        _internalSlider.Value += 0.05f;

        _vm.AudioManager.Playback.Pitch += 0.05f;

        Save(_vm.AudioManager.Playback.Pitch);
    }

    private static void OnPitchDownTriggered(object? sender, EventArgs e)
    {
        _internalSlider.Value -= 0.05f;

        _vm.AudioManager.Playback.Pitch -= 0.05f;

        Save(_vm.AudioManager.Playback.Pitch);
    }

    private static string GetToolTip()
    {
        return $"{_vm.SettingsService.Settings.Pitch}";
    }
}