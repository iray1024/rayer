using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Rayer.Core.PInvoke;

internal static partial class Win32
{
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