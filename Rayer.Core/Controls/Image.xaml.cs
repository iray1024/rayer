using System.Windows;
using System.Windows.Controls;

namespace Rayer.Core.Controls;

public partial class Image : UserControl
{
    public Image()
    {
        InitializeComponent();
    }

    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
        "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Image));

    public event RoutedEventHandler Click
    {
        add { AddHandler(ClickEvent, value); }
        remove { RemoveHandler(ClickEvent, value); }
    }

    private static void ClickCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Image image)
        {
            image.RaiseEvent(new RoutedEventArgs(ClickEvent, d));
        }
    }
}