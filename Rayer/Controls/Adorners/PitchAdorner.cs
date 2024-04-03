using Rayer.Core.Abstractions;
using Rayer.ViewModels;
using System.Runtime.CompilerServices;
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

        SetInternalControls();

        ToolTipService.SetToolTip(_internalImage, "重置音频");
        ToolTipService.SetToolTip(_internalSlider, GetToolTip());

        AddVisualChild(_panel);

        ApplicationThemeManager.Changed += OnThemeChanged;
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
                    image.Source = (ImageSource)Application.Current.Resources["Pitch"];

                    _internalImage = image;
                }
                if (item is Slider slider && slider.Name == "PitchSlider")
                {
                    slider.MouseWheel += OnPitchMouseWheel;
                    slider.PreviewMouseDown += OnPreviewMouseDown;

                    slider.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnDragDelta), true);
                    slider.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnDragCompleted), true);

                    _internalSlider = slider;
                }
            }
        }
    }

    private void OnPitchMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var factor = _vm.AudioManager.Playback.Pitch + (e.Delta / 1000.0f);

        var percentage = Math.Min(Math.Max(TransformFromPercentage(factor), 0), 1);

        _internalSlider.Value = percentage * 100;
        _vm.AudioManager.Playback.Pitch = factor;
        Save(factor);
    }

    private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        var percentage = (float)(e.GetPosition(_internalSlider).X / _internalSlider.ActualWidth * (_internalSlider.Maximum - _internalSlider.Minimum));

        _internalSlider.Value = percentage;

        var factor = TransformFromPercentage(percentage);

        _vm.AudioManager.Playback.Pitch = factor;
        Save(factor);
    }

    private void OnDragDelta(object sender, DragDeltaEventArgs e)
    {
        var factor = TransformFromPercentage((float)_internalSlider.Value);

        _vm.AudioManager.Playback.Pitch = factor;
    }

    private void OnDragCompleted(object sender, DragCompletedEventArgs e)
    {
        var factor = TransformFromPercentage((float)_internalSlider.Value);

        _vm.AudioManager.Playback.Pitch = factor;

        Save(factor);
    }

    private static void Save(float factor)
    {
        ToolTipService.SetToolTip(_internalSlider, GetToolTip());

        _vm.SettingsService.Settings.Pitch = MathF.Round(factor, 2);
        _vm.SettingsService.Save();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float TransformFromFactor(float factor)
    {
        return (factor - 0.5f) * (100 / 1.5f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float TransformFromPercentage(float percentage)
    {
        return 0.5f + (percentage / 100 * 1.5f);
    }
    #endregion

    private static void OnPitchUpTriggered(object? sender, EventArgs e)
    {
        var percentage = TransformFromFactor(0.55f);

        _internalSlider.Value += percentage;

        _vm.AudioManager.Playback.Pitch += 0.05f;

        Save(_vm.AudioManager.Playback.Pitch);
    }

    private static void OnPitchDownTriggered(object? sender, EventArgs e)
    {
        var percentage = TransformFromFactor(0.55f);

        _internalSlider.Value -= percentage;

        _vm.AudioManager.Playback.Pitch -= 0.05f;

        Save(_vm.AudioManager.Playback.Pitch);
    }

    private static string GetToolTip()
    {
        return $"{_vm.SettingsService.Settings.Pitch}";
    }
}