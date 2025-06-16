using Microsoft.Xaml.Behaviors.Media;
using System.Windows;
using System.Windows.Media.Effects;

namespace Rayer.Core.Effects;

public sealed class SmoothSwirlGridTransitionEffect : TransitionEffect
{
    private const double DefaultCellCount = 10.0;
    private const double DefaultTwistAmount = 10.0;

    public static readonly DependencyProperty TwistAmountProperty =
        DependencyProperty.Register(
            nameof(TwistAmount),
            typeof(double),
            typeof(SmoothSwirlGridTransitionEffect),
            new PropertyMetadata(10.0, PixelShaderConstantCallback(1)));

    public static readonly DependencyProperty CellCountProperty =
        DependencyProperty.Register(
            nameof(CellCount),
            typeof(double),
            typeof(SmoothSwirlGridTransitionEffect),
            new PropertyMetadata(10.0, PixelShaderConstantCallback(2)));

    public SmoothSwirlGridTransitionEffect(double twist)
      : this()
    {
        TwistAmount = twist;
    }

    public SmoothSwirlGridTransitionEffect()
    {
        PixelShader = new PixelShader()
        {
            UriSource = new Uri("pack://application:,,,/Rayer.Core;component/Effects/Shaders/SmoothSwirlGridTransitioneffect.ps")
        };

        UpdateShaderValue(TwistAmountProperty);
        UpdateShaderValue(CellCountProperty);
    }

    public double TwistAmount
    {
        get => (double)GetValue(TwistAmountProperty);
        set => SetValue(TwistAmountProperty, value);
    }

    public double CellCount
    {
        get => (double)GetValue(CellCountProperty);
        set => SetValue(CellCountProperty, value);
    }

    protected override TransitionEffect DeepCopy()
    {
        return new SmoothSwirlGridTransitionEffect()
        {
            TwistAmount = TwistAmount,
            CellCount = CellCount
        };
    }
}