using Rayer.FFmpegCore.Interops;
using System.Runtime.InteropServices;

namespace Rayer.FFmpegCore;

/// <summary>
/// Provides data for the <see cref="FFmpegUtils.FFmpegLogReceived"/> event.
/// </summary>
public class FFmpegLogReceivedEventArgs : EventArgs
{
    internal unsafe FFmpegLogReceivedEventArgs(AVClass? avClass, AVClass? parentLogContext, LogLevel level, string line, void* ptr, void* ptr1)
    {
        ItemNameFunc itemNameFunc;
        nint strPtr;

        Message = line;
        Level = level;

        if (avClass != null)
        {
            var avc = avClass.Value;

            ClassName = Marshal.PtrToStringAnsi((nint)avc.class_name) ?? string.Empty;
            if (avc.item_name != nint.Zero)
            {
                itemNameFunc = (ItemNameFunc)Marshal.GetDelegateForFunctionPointer(avc.item_name, typeof(ItemNameFunc));
                strPtr = itemNameFunc((nint)ptr);
                if (strPtr != nint.Zero)
                {
                    ItemName = Marshal.PtrToStringAnsi(strPtr) ?? string.Empty;
                }
            }
        }
        if (parentLogContext != null)
        {
            var pavc = parentLogContext.Value;

            ParentLogContextClassName = Marshal.PtrToStringAnsi((nint)pavc.class_name) ?? string.Empty;
            if (pavc.item_name != nint.Zero)
            {
                itemNameFunc = (ItemNameFunc)Marshal.GetDelegateForFunctionPointer(pavc.item_name, typeof(ItemNameFunc));
                strPtr = itemNameFunc((nint)ptr1);
                if (strPtr != nint.Zero)
                {
                    ParentLogContextItemName = Marshal.PtrToStringAnsi(strPtr) ?? string.Empty;
                }
            }
        }
    }

    /// <summary>
    /// Gets the message.
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    public string Message { get; private set; }

    /// <summary>
    /// Gets the level of the message.
    /// </summary>
    /// <value>
    /// The level of the message.
    /// </value>
    public LogLevel Level { get; private set; }

    /// <summary>
    /// Gets the name of the class.
    /// </summary>
    /// <value>
    /// The name of the class.
    /// </value>
    public string ClassName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the item name of the class.
    /// </summary>
    /// <value>
    /// The item name of the class.
    /// </value>
    public string ItemName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the parent log context class.
    /// </summary>
    /// <value>
    /// The name of the parent log context class. Might me empty.
    /// </value>
    public string ParentLogContextClassName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the item name of the parent log context class.
    /// </summary>
    /// <value>
    /// The item name of the parent log context class. Might me empty.
    /// </value>
    public string ParentLogContextItemName { get; set; } = string.Empty;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint ItemNameFunc(nint avClass);
}