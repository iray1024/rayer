using System.ComponentModel;
using System.Windows;

namespace Rayer.Core.Controls;

public class ListViewItem : System.Windows.Controls.ListViewItem
{
    public static new readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(
            "IsSelected",
            typeof(bool),
            typeof(ListViewItem),
            new PropertyMetadata(false));

    public new bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public static readonly DependencyProperty IsAvailableProperty =
        DependencyProperty.Register(
            "IsAvailable",
            typeof(bool),
            typeof(ListViewItem),
            new PropertyMetadata(true));

    [Bindable(true)]
    public bool IsAvailable
    {
        get => (bool)GetValue(IsAvailableProperty);
        set => SetValue(IsAvailableProperty, value);
    }
}