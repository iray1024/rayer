using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using static Rayer.Core.PInvoke.Win32;
using static Rayer.Core.PInvoke.Win32.User32;

namespace Rayer.Core.Utils;

public static class ElementHelper
{
    public static void TraverseControlsAndSetNoneFocusable(DependencyObject root)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);

            if (child is Control control)
            {
                if (control.Name == "AutoSuggest")
                {
                    continue;
                }

                control.IsTabStop = false;
            }

            TraverseControlsAndSetNoneFocusable(child);
        }
    }

    public static void FullScreen(Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;

        var lStyle = GetWindowLong(hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE);

        _ = SetWindowLong(hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE, 369557504);

        ShowWindow(hwnd, SHOW_WINDOW_CMD.SW_MAXIMIZE);
    }

    public static void EndFullScreen(Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;

        var lStyle = GetWindowLong(hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE);

        _ = SetWindowLong(hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE, lStyle | (int)WINDOW_STYLE.WS_CAPTION);

        ShowWindow(hwnd, SHOW_WINDOW_CMD.SW_RESTORE);
    }
}