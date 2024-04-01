using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Rayer.Core.Behaviors;

public static class AttachCommandBehavior
{
    public static readonly DependencyProperty AttachCommandProperty = DependencyProperty.RegisterAttached(
        "AttachCommand", typeof(ICommand), typeof(AttachCommandBehavior), new PropertyMetadata(null, OnAttachCommandChanged));

    public static void AttachClickCommand(DependencyObject element, ICommand value)
    {
        element.SetValue(AttachCommandProperty, value);
    }

    public static ICommand GetAttachCommand(DependencyObject element)
    {
        return (ICommand)element.GetValue(AttachCommandProperty);
    }

    private static void OnAttachCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement el)
        {
            return;
        }

        if (e.OldValue != null)
        {
            el.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(DefaultHandler));
        }

        if (e.NewValue != null)
        {
            el.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(DefaultHandler));
        }
    }

    private static void DefaultHandler(object sender, RoutedEventArgs e)
    {
        if (sender is UIElement el)
        {
            var command = GetAttachCommand(el);

            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }
        }
    }
}