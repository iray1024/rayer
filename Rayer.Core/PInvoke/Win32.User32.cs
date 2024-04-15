using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rayer.Core.PInvoke;

internal static partial class Win32
{
    internal static partial class User32
    {
        public const string LibraryName = "user32";

        [LibraryImport("user32.dll", EntryPoint = "GetWindowLongA")]
        internal static partial int GetWindowLong(IntPtr hWnd, int nIndex);

        [LibraryImport("user32.dll", EntryPoint = "SetWindowLongA")]
        internal static partial int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public const uint MONITOR_DEFAULTTONEAREST = 2;
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOZORDER = 0x0004;

        [LibraryImport("user32.dll")]
        internal static partial IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TOOLWINDOW = 0x00000080;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [LibraryImport("user32.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        internal static partial int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [LibraryImport("user32.dll", EntryPoint = "SetWindowsHookExA")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
        internal static partial int SetWindowsHookEx(int idHook, KeyboardHookProc lpfn, IntPtr hModule, int dwThreadId);

        [LibraryImport("user32.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool UnhookWindowsHookEx(int idHook);

        public delegate int KeyboardHookProc(int nCode, int wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        internal class KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
    }
}