using Microsoft.Xaml.Behaviors.Media;
using Rayer.Core.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Rayer.Core.Effects;

public sealed class ImageTransition : DependencyObject
{
    private static Guid _currentTransitionId;

    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.RegisterAttached(
            "Source",
            typeof(ImageSource),
            typeof(ImageTransition),
            new PropertyMetadata(null, OnSourceChanged));

    public static ImageSource GetSource(DependencyObject obj) => (ImageSource)obj.GetValue(SourceProperty);
    public static void SetSource(DependencyObject obj, ImageSource value) => obj.SetValue(SourceProperty, value);

    public static void ForceRenderEffect(DependencyObject obj)
    {
        if (obj is not AsyncImage image)
        {
            return;
        }

        var source = GetSource(obj);

        RenderEffect(image, source);
    }

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not AsyncImage image)
        {
            return;
        }

        var oldImage = (ImageSource)e.OldValue;
        var newImage = (ImageSource)e.NewValue;

        if (oldImage == null || newImage == null)
        {
            image.Source = newImage;
            return;
        }

        RenderEffect(image, newImage);
    }

    private static void RenderEffect(AsyncImage image, ImageSource source)
    {
        var transitionId = Guid.NewGuid();
        _currentTransitionId = transitionId;

        var effect = new RadialBlurTransitionEffect();
        effect.Input = new ImageBrush(source);
        effect.Progress = 0;

        RenderOptions.SetCachingHint(effect, CachingHint.Cache);
        RenderOptions.SetCacheInvalidationThresholdMinimum(effect, 0.5);

        image.Effect = effect;

        var animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2))
        {
            EasingFunction = new BackEase()
            {
                EasingMode = EasingMode.EaseInOut
            }
        };

        animation.Completed += (s, _) =>
        {
            if (transitionId == _currentTransitionId)
            {
                image.Effect = null;
            }
        };

        effect.BeginAnimation(TransitionEffect.ProgressProperty, animation);
    }
}