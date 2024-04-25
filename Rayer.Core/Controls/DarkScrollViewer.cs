using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rayer.Core.Controls;

public class DarkScrollViewer : ScrollViewer
{
    public static readonly DependencyProperty IsScrollSpillEnabledProperty = DependencyProperty.Register(
        nameof(IsScrollSpillEnabled),
        typeof(bool),
        typeof(DarkScrollViewer),
        new PropertyMetadata(true)
    );

    public bool IsScrollSpillEnabled
    {
        get { return (bool)GetValue(IsScrollSpillEnabledProperty); }
        set { SetValue(IsScrollSpillEnabledProperty, value); }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        if (IsVerticalScrollingDisabled ||
            IsContentSmallerThanViewport ||
            (IsScrollSpillEnabled && HasReachedEndOfScrolling(e)))
        {
            return;
        }

        base.OnMouseWheel(e);
    }

    private bool IsVerticalScrollingDisabled => VerticalScrollBarVisibility == ScrollBarVisibility.Disabled;

    private bool IsContentSmallerThanViewport => ScrollableHeight <= 0;

    private bool HasReachedEndOfScrolling(MouseWheelEventArgs e)
    {
        var isScrollingUp = e.Delta > 0;
        var isScrollingDown = e.Delta < 0;
        var isTopOfViewport = VerticalOffset == 0;
        var isBottomOfViewport = VerticalOffset >= ScrollableHeight;

        return (isScrollingUp && isTopOfViewport) || (isScrollingDown && isBottomOfViewport);
    }
}