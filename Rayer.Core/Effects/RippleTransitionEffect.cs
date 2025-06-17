using Microsoft.Xaml.Behaviors.Media;
using System.Windows.Media.Effects;

namespace Rayer.Core.Effects;

public sealed class RippleTransitionEffect : TransitionEffect
{
    public RippleTransitionEffect()
    {
        PixelShader = new PixelShader()
        {
            UriSource = new Uri("pack://application:,,,/Rayer.Core;component/Effects/Shaders/RippleTransitionEffect.ps")
        };
    }

    protected override TransitionEffect DeepCopy() => new RippleTransitionEffect();
}