using System.Windows;
using System.Windows.Controls;

namespace Rayer.Core.Common;

public class EmptyListTemplateSelector : DataTemplateSelector
{
    public DataTemplate EmptyTemplate { get; set; } = null!;
    public DataTemplate NormalTemplate { get; set; } = null!;

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        return item is not null ? EmptyTemplate : NormalTemplate;
    }
}