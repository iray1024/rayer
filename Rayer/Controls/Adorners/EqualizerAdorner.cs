﻿using Rayer.Abstractions;
using Rayer.Core.AudioEffect.Abstractions;
using Rayer.Core.Framework;
using Rayer.Markup;
using Rayer.Services;
using Rayer.Views.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Rayer.Controls.Adorners;

internal class EqualizerAdorner : Adorner
{
    private readonly IImmersivePlayerService _immersivePlayerService;

    private readonly ImageIcon _equalizer = default!;

    public EqualizerAdorner(UIElement adornedElement)
        : base(adornedElement)
    {
        _immersivePlayerService = App.GetRequiredService<IImmersivePlayerService>();

        _immersivePlayerService.Show += OnSwitchImmersivePlayerDisplay;
        _immersivePlayerService.Hidden += OnSwitchImmersivePlayerDisplay;

        _equalizer = new ImageIcon
        {
            Width = 24,
            Height = 24,
            HorizontalAlignment = HorizontalAlignment.Right,
            Cursor = Cursors.Hand,
            Source = GetSource(),
        };

        var equalizerProvider = App.GetRequiredService<IEqualizerProvider>();

        if (equalizerProvider.Available)
        {
            _equalizer.MouseUp += OnEqualizerMouseUp;

            ToolTipService.SetToolTip(_equalizer, GetToolTip());
        }
        else
        {
            _equalizer.ToolTip = "缺少均衡器参数文件";
        }

        RenderOptions.SetBitmapScalingMode(_equalizer, BitmapScalingMode.Fant);
        AddVisualChild(_equalizer);

        ApplicationThemeManager.Changed += OnThemeChanged;
    }

    protected override int VisualChildrenCount => 1;

    protected override Visual GetVisualChild(int index)
    {
        return _equalizer;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _equalizer.Arrange(new Rect(
            new Point(AdornedElement.DesiredSize.Width - (_equalizer.ActualWidth * 3.8), _equalizer.ActualHeight + 3),
            _equalizer.DesiredSize));

        return finalSize;
    }

    private void OnEqualizerMouseUp(object sender, MouseButtonEventArgs e)
    {
        var wndProvider = App.GetRequiredService<IWindowsProviderService>();

        wndProvider.Show<EqualizerWindow>();
    }

    private void OnSwitchImmersivePlayerDisplay(object? sender, EventArgs e)
    {
        _equalizer.Source = GetSource();
    }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        _equalizer.Source = GetSource();
    }

    private ImageSource GetSource()
    {
        return _immersivePlayerService.IsNowImmersive
            ? StaticThemeResources.Dark.Equalizer
            : (ImageSource)StaticThemeResources.GetDynamicResource(nameof(ThemeSymbol.Equalizer));
    }

    private static string GetToolTip()
    {
        return $"均衡器";
    }
}