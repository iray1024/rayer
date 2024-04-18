using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Rayer.SearchEngine.Internal.Effects;

internal class GrayscaleBitmapEffect : ShaderEffect
{
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(
            "Input",
            typeof(GrayscaleBitmapEffect),
            0);

    public GrayscaleBitmapEffect()
    {
        PixelShader = new PixelShader
        {
            UriSource = new Uri("pack://application:,,,/Rayer.SearchEngine;component/Internal/Effects/GrayscaleEffect.ps")
        };

        UpdateShaderValue(InputProperty);
    }

    public Brush Input
    {
        get => (Brush)GetValue(InputProperty);
        set => SetValue(InputProperty, value);
    }
}