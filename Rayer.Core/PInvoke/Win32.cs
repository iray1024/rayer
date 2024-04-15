using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Rayer.Core.PInvoke;

internal static partial class Win32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    public enum WINDOW_LONG_PTR_INDEX
    {
        GWL_STYLE = -16
    }

    public enum WINDOW_STYLE
    {
        WS_CAPTION = 0x00C00000
    }

    public enum SHOW_WINDOW_CMD
    {
        SW_MAXIMIZE = 3,
        SW_RESTORE = 9,
    }

    [LibraryImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [SupportedOSPlatform("windows5.0")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ShowWindow(IntPtr hWnd, SHOW_WINDOW_CMD nCmdShow);
}