using System.Windows;

namespace Rayer.Core.Behaviors;

public static class AttachClickBehavior
{
    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
        "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AttachClickBehavior));

    

    public static void AddClickHandler(DependencyObject d, RoutedEventHandler handler)
    {
        var element = d as UIElement;

        element?.AddHandler(ClickEvent, handler);
    }

    public static void RemoveClickHandler(DependencyObject d, RoutedEventHandler handler)
    {
        var element = d as UIElement;

        element?.RemoveHandler(ClickEvent, handler);
    }

    private static void RaiseClickEvent(UIElement element)
    {
        var args = new RoutedEventArgs(ClickEvent);

        element.RaiseEvent(args);
    }
}