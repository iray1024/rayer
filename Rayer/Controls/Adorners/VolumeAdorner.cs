using Rayer.Abstractions;
using Rayer.Services;
using Rayer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Rayer.Controls;

public class VolumeAdorner : Adorner
{
    private readonly PlaybarViewModel _vm;
    private readonly IImmersivePlayerService _immersivePlayerService;

    private readonly VolumePanel _panel = default!;

    public VolumeAdorner(UIElement adornedElement)
        : base(adornedElement)
    {
        _vm = App.GetRequiredService<PlaybarViewModel>();

        _immersivePlayerService = App.GetRequiredService<IImmersivePlayerService>();

        _immersivePlayerService.Show += OnSwicthImmersivePlayerDisplay;
        _immersivePlayerService.Hidden += OnSwicthImmersivePlayerDisplay;

        _panel = new VolumePanel();

        SetInternalImageIcon();
        ToolTipService.SetToolTip(_panel, GetToolTip());

        AddVisualChild(_panel);

        ApplicationThemeManager.Changed += OnThemeChanged;


    }

    private void OnSwicthImmersivePlayerDisplay(object? sender, EventArgs e)
    {
        SetInternalImageIcon();
    }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        SetInternalImageIcon();
    }

    protected override int VisualChildrenCount => 1;

    protected override Visual GetVisualChild(int index)
    {
        return _panel;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _panel.Arrange(new Rect(
            new Point(AdornedElement.DesiredSize.Width + 28, 1),
            _panel.DesiredSize));

        return finalSize;
    }

    #region Volume    
    private void SetInternalImageIcon()
    {
        if (_panel.Content is Grid grid)
        {
            foreach (var item in grid.Children)
            {
                if (item is ImageIcon image && image.Name == "Volume")
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
            ? _vm.SettingsService.Settings.Volume == 0f
                ? StaticThemeResources.Dark.Mute
                : _vm.SettingsService.Settings.Volume > 0.5f
                    ? StaticThemeResources.Dark.VolumeFull
                    : StaticThemeResources.Dark.VolumeHalf
            : _vm.SettingsService.Settings.Volume == 0f
                ? (ImageSource)Application.Current.Resources["Mute"]
                : _vm.SettingsService.Settings.Volume > 0.5f
                    ? (ImageSource)Application.Current.Resources["VolumeFull"]
                    : (ImageSource)Application.Current.Resources["VolumeHalf"];
    }

    private string GetToolTip()
    {
        return $"音量：{(int)(_vm.SettingsService.Settings.Volume * 100)}%";
    }
    #endregion
}