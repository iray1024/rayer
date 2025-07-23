using Rayer.Abstractions;
using Rayer.Core.Common;
using Rayer.Services;
using Rayer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Rayer.Controls.Adorners;

public class PlayloopAdorner : Adorner
{
    private readonly PlaybarViewModel _vm;
    private readonly IImmersivePlayerService _immersivePlayerService;

    private readonly ImageIcon _playLoop = default!;
    private static readonly int _playLoopMaxValue = (int)Enum.GetValues<PlayloopMode>().Max();

    public PlayloopAdorner(UIElement adornedElement)
        : base(adornedElement)
    {
        _vm = App.GetRequiredService<PlaybarViewModel>();

        _immersivePlayerService = App.GetRequiredService<IImmersivePlayerService>();

        _immersivePlayerService.Show += OnSwitchImmersivePlayerDisplay;
        _immersivePlayerService.Hidden += OnSwitchImmersivePlayerDisplay;

        _playLoop = new ImageIcon
        {
            Width = 24,
            Height = 24,
            HorizontalAlignment = HorizontalAlignment.Right,
            Cursor = Cursors.Hand,
            Source = GetSource(),
        };

        _playLoop.MouseUp += OnPlayloopMouseUp;

        RenderOptions.SetBitmapScalingMode(_playLoop, BitmapScalingMode.Fant);
        ToolTipService.SetToolTip(_playLoop, GetToolTip());

        AddVisualChild(_playLoop);

        ApplicationThemeManager.Changed += OnThemeChanged;
    }

    private void OnSwitchImmersivePlayerDisplay(object? sender, EventArgs e)
    {
        _playLoop.Source = GetSource();
    }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        _playLoop.Source = GetSource();
    }

    protected override int VisualChildrenCount => 1;

    protected override Visual GetVisualChild(int index)
    {
        return _playLoop;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _playLoop.Arrange(new Rect(
            new Point(0 - _playLoop.ActualWidth - 28, 4),
            _playLoop.DesiredSize));

        return finalSize;
    }

    #region Playloop    
    private void OnPlayloopMouseUp(object sender, MouseButtonEventArgs e)
    {
        var currentPlayloopMode = (int)_vm.SettingsService.Settings.PlayloopMode;

        currentPlayloopMode = (currentPlayloopMode + 1) % (_playLoopMaxValue + 1);

        _vm.SettingsService.Settings.PlayloopMode = (PlayloopMode)currentPlayloopMode;
        _vm.SettingsService.Save();

        _vm.AudioManager.Playback.SetPlayMode(_vm.SettingsService.Settings.PlayloopMode);

        _playLoop.Source = GetSource();

        ToolTipService.SetToolTip(_playLoop, GetToolTip());
    }

    private ImageSource GetSource()
    {
        return _immersivePlayerService.IsNowImmersive
            ? _vm.SettingsService.Settings.PlayloopMode switch
            {
                PlayloopMode.Shuffle => StaticThemeResources.Dark.Shuffle,
                PlayloopMode.List => StaticThemeResources.Dark.Repeat,
                PlayloopMode.Single => StaticThemeResources.Dark.Single,
                _ => StaticThemeResources.Dark.Repeat,
            }
            : (ImageSource)(_vm.SettingsService.Settings.PlayloopMode switch
            {
                PlayloopMode.Shuffle => Application.Current.Resources["Shuffle"],
                PlayloopMode.List => Application.Current.Resources["Repeat"],
                PlayloopMode.Single => Application.Current.Resources["Single"],
                _ => Application.Current.Resources["Repeat"],
            });
    }

    private string GetToolTip()
    {
        return _vm.SettingsService.Settings.PlayloopMode switch
        {
            PlayloopMode.Shuffle => "随机播放",
            PlayloopMode.List => "列表循环",
            PlayloopMode.Single => "单曲循环",
            _ => "列表循环",
        };
    }
    #endregion
}