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

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);


    [LibraryImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DeleteObject(IntPtr hObject);

    [LibraryImport("dwmapi.dll")]
    public static partial int DwmSetIconicThumbnail(IntPtr hwnd, IntPtr hbmp, int dwSITFlags);

    [LibraryImport("dwmapi.dll")]
    public static partial int DwmInvalidateIconicBitmaps(IntPtr hwnd);

    [LibraryImport("dwmapi.dll")]
    public static partial int DwmSetIconicLivePreviewBitmap(IntPtr hwnd, IntPtr hBitmap, IntPtr pptClient, DWM_SIT dwSITFlags);

    [LibraryImport("dwmapi.dll")]
    public static partial int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttributes dwAttribute, IntPtr pvAttribute, int cbAttribute);

    [Flags]
    public enum DwmWindowAttributes : uint
    {
        None = 0,
        DISPLAYFRAME = 1,
        FORCE_ICONIC_REPRESENTATION = 7,
        HAS_ICONIC_BITMAP = 10
    }

    [Flags]
    public enum DWM_SIT : uint
    {
        None = 0x0,
        DisplayFrame = 0x1
    }
}