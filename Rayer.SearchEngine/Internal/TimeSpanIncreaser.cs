using System.Windows;

namespace Rayer.SearchEngine.Internal;

public class TimeSpanIncreaser : Increaser<TimeSpan>
{
    public static readonly DependencyProperty StartProperty =
        DependencyProperty.Register("Start", typeof(TimeSpan), typeof(TimeSpanIncreaser),
            new PropertyMetadata(default(TimeSpan)));

    private TimeSpan _current;

    public override TimeSpan Next
    {
        get
        {
            var result = Start + _current;
            _current += Step;
            return result;
        }
    }

    public override TimeSpan Start
    {
        get => (TimeSpan)GetValue(StartProperty);
        set => SetValue(StartProperty, value);
    }
}

public abstract class Increaser<T> : DependencyObject
{
    public abstract T Next { get; }

    public virtual T Start { get; set; } = default!;

    public T Step { get; set; } = default!;
}