using System.Windows;

namespace Rayer.Core.Controls;

public class ListView : System.Windows.Controls.ListView
{
    static ListView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ListView),
            new FrameworkPropertyMetadata(typeof(ListView))
        );
    }

    public ListView()
    {

    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new ListViewItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is ListViewItem;
    }
}