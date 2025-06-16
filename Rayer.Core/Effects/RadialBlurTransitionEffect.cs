using Microsoft.Xaml.Behaviors.Media;
using System.Windows.Media.Effects;

namespace Rayer.Core.Effects;

public class RadialBlurTransitionEffect : TransitionEffect
{
    public RadialBlurTransitionEffect()
    {
        PixelShader = new PixelShader()
        {
            UriSource = new Uri("pack://application:,,,/Rayer.Core;component/Effects/Shaders/RadialBlurTransitionEffect.ps")
        };
    }

    protected override TransitionEffect DeepCopy()
    {
        return new RadialBlurTransitionEffect();
    }
}