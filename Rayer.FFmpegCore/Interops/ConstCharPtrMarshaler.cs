using System.Runtime.InteropServices;

namespace Rayer.FFmpegCore.Interops;

internal class ConstCharPtrMarshaler : ICustomMarshaler
{
    public object MarshalNativeToManaged(nint pNativeData)
    {
        return Marshal.PtrToStringAnsi(pNativeData)!;
    }

    public nint MarshalManagedToNative(object managedObj)
    {
        return nint.Zero;
    }

    public void CleanUpNativeData(nint pNativeData)
    {
    }

    public void CleanUpManagedData(object managedObj)
    {
    }

    public int GetNativeDataSize()
    {
        return nint.Size;
    }

    private static readonly ConstCharPtrMarshaler Instance = new();

    public static ICustomMarshaler GetInstance(string cookie)
    {
        return Instance;
    }
}