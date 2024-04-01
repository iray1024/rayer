using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rayer.Core.Utils;

public class ElementHelper
{
    public static void TraverseControlsAndSetNoneFocusable(DependencyObject root)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);

            if (child is Control control)
            {
                control.IsTabStop = false;

                if (control is UIElement element)
                {
                    element.Focusable = false;
                }
            }

            TraverseControlsAndSetNoneFocusable(child);
        }
    }
}