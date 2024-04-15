using System.Runtime.InteropServices;

namespace Rayer.FFmpegCore.Interops;

internal static partial class InteropHelper
{
    public const string LD_LIBRARY_PATH = "LD_LIBRARY_PATH";

    public static void RegisterLibrariesSearchPath(string path)
    {
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
                SetDllDirectory(path);
                break;
            case PlatformID.Unix:
            case PlatformID.MacOSX:
                var currentValue = Environment.GetEnvironmentVariable(LD_LIBRARY_PATH);
                if (string.IsNullOrEmpty(currentValue) == false && currentValue.Contains(path) == false)
                {
                    var newValue = currentValue + Path.PathSeparator + path;
                    Environment.SetEnvironmentVariable(LD_LIBRARY_PATH, newValue);
                }
                break;
        }
    }

    [LibraryImport("kernel32", EntryPoint = "SetDllDirectoryW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetDllDirectory(string lpPathName);
}