using Microsoft.Xaml.Behaviors.Media;
using System.Windows;
using System.Windows.Media.Effects;

namespace Rayer.Core.Effects;

public sealed class WaveTransitionEffect : TransitionEffect
{
    private const double DefaultMagnitude = 0.1;
    private const double DefaultPhase = 14.0;
    private const double DefaultFrequency = 20.0;
    public static readonly DependencyProperty MagnitudeProperty =
        DependencyProperty.Register(nameof(Magnitude), typeof(double), typeof(WaveTransitionEffect), new PropertyMetadata(0.1, PixelShaderConstantCallback(1)));

    public static readonly DependencyProperty PhaseProperty =
        DependencyProperty.Register(nameof(Phase), typeof(double), typeof(WaveTransitionEffect), new PropertyMetadata(14.0, PixelShaderConstantCallback(2)));

    public static readonly DependencyProperty FrequencyProperty =
        DependencyProperty.Register(nameof(Frequency), typeof(double), typeof(WaveTransitionEffect), new PropertyMetadata(20.0, PixelShaderConstantCallback(3)));

    public double Magnitude
    {
        get => (double)GetValue(MagnitudeProperty);
        set => SetValue(MagnitudeProperty, value);
    }

    public double Phase
    {
        get => (double)GetValue(PhaseProperty);
        set => SetValue(PhaseProperty, value);
    }

    public double Frequency
    {
        get => (double)GetValue(FrequencyProperty);
        set => SetValue(FrequencyProperty, value);
    }

    public WaveTransitionEffect()
    {
        PixelShader = new PixelShader()
        {
            UriSource = new Uri("pack://application:,,,/Rayer.Core;component/Effects/Shaders/WaveTransitionEffect.ps")
        };

        UpdateShaderValue(MagnitudeProperty);
        UpdateShaderValue(PhaseProperty);
        UpdateShaderValue(FrequencyProperty);
    }

    protected override TransitionEffect DeepCopy()
    {
        return new WaveTransitionEffect()
        {
            Magnitude = Magnitude,
            Phase = Phase,
            Frequency = Frequency
        };
    }
}