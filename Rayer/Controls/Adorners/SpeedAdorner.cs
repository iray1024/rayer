using Rayer.Abstractions;
using Rayer.Core.Abstractions;
using Rayer.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Rayer.Controls.Adorners;

internal class SpeedAdorner : Adorner
{
    private readonly IAudioManager _audioManager;
    private readonly IImmersivePlayerService _immersivePlayerService;

    private readonly SpeedPanel _panel = default!;

    public SpeedAdorner(UIElement adornedElement)
        : base(adornedElement)
    {
        _audioManager = App.GetRequiredService<IAudioManager>();
        _immersivePlayerService = App.GetRequiredService<IImmersivePlayerService>();

        _immersivePlayerService.Show += OnSwicthImmersivePlayerDisplay;
        _immersivePlayerService.Hidden += OnSwicthImmersivePlayerDisplay;

        _panel = new SpeedPanel();

        SetInternalImageIcon();

        AddVisualChild(_panel);

        ApplicationThemeManager.Changed += OnThemeChanged;

        ToolTipService.SetToolTip(_panel, $"速度：{(int)(_audioManager.Playback.DeviceManager.Speed * 100)}%");
    }

    protected override int VisualChildrenCount => 1;

    protected override Visual GetVisualChild(int index)
    {
        return _panel;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _panel.Arrange(new Rect(
            new Point(AdornedElement.DesiredSize.Width + 60, 1),
            _panel.DesiredSize));

        return finalSize;
    }

    private void OnSwicthImmersivePlayerDisplay(object? sender, EventArgs e)
    {
        SetInternalImageIcon();
    }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        SetInternalImageIcon();
    }

    private void SetInternalImageIcon()
    {
        if (_panel.Content is Grid grid)
        {
            foreach (var item in grid.Children)
            {
                if (item is ImageIcon image && image.Name == "Speed")
                {
                    RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
                    SetVolumeElement(image);
                }
            }
        }
    }

    private void SetVolumeElement(ImageIcon image)
    {
        image.Source = _immersivePlayerService.IsNowImmersive
            ? StaticThemeResources.Dark.Speed
            : ApplicationThemeManager.GetAppTheme() is ApplicationTheme.Dark
                ? StaticThemeResources.Dark.Speed
                : StaticThemeResources.Light.Speed;
    }
}