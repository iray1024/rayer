using Rayer.FFmpegCore.Interops;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Rayer.FFmpegCore;

/// <summary>
/// Contains some utilities for working with FFmpeg.
/// </summary>
public static class FFmpegUtils
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private static readonly FFmpegCalls.LogCallback LogCallback;
    private static readonly FFmpegCalls.LogCallback DefaultLogCallback;
    private static readonly object LockObj = new();

    /// <summary>
    /// Occurs when a FFmpeg log entry was received.
    /// </summary>
    public static event EventHandler<FFmpegLogReceivedEventArgs>? FFmpegLogReceived;

    /// <summary>
    /// Occurs when the location of the native FFmpeg binaries has get resolved.
    /// Note: This is currently only available for Windows Platforms.
    /// </summary>
    public static event EventHandler<ResolveFFmpegAssemblyLocationEventArgs>? ResolveFfmpegAssemblyLocation;

    static unsafe FFmpegUtils()
    {
        LogCallback = OnLogMessage;
        DefaultLogCallback = FFmpegCalls.GetDefaultLogCallback();
        FFmpegCalls.SetLogCallback(LogCallback);
    }

    /// <summary>
    /// Gets the output formats.
    /// </summary>
    /// <returns>All supported output formats.</returns>
    public static IEnumerable<Format> GetOutputFormats()
    {
        var outputFormats = FFmpegCalls.GetOutputFormats();
        return outputFormats.Select(format => new Format(format));
    }

    /// <summary>
    /// Gets the input formats.
    /// </summary>
    /// <returns>All supported input formats.</returns>
    public static IEnumerable<Format> GetInputFormats()
    {
        var inputFormats = FFmpegCalls.GetInputFormats();
        return inputFormats.Select(format => new Format(format));
    }

    /// <summary>
    /// Gets or sets the log level.
    /// </summary>
    /// <value>
    /// The log level.
    /// </value>
    /// <exception cref="InvalidEnumArgumentException">value</exception>
    public static LogLevel LogLevel
    {
        get { return FFmpegCalls.GetLogLevel(); }
        set
        {
            if ((int)value < (int)LogLevel.Quit || (int)value > (int)LogLevel.Debug || (int)value % 8 != 0)
            {
                throw new InvalidEnumArgumentException("value", (int)value, typeof(LogLevel));
            }

            FFmpegCalls.SetLogLevel(value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether log entries should be passed to the default FFmpeg logger.
    /// </summary>
    /// <value>
    ///   <c>true</c> if log messages should be passed to the default FFmpeg logger; otherwise, <c>false</c>.
    /// </value>
    public static bool LogToDefaultLogger { get; set; }

    private static unsafe void OnLogMessage(void* ptr, int level, byte* fmt, nint vl)
    {
        lock (LockObj)
        {
            if (level >= 0)
            {
                level &= 0xFF;
            }

            if (level > (int)LogLevel)
            {
                return;
            }

            if (LogToDefaultLogger)
            {
                DefaultLogCallback(ptr, level, fmt, vl);
            }

            var eventHandler = FFmpegLogReceived;
            if (eventHandler != null)
            {
                AVClass? avClass = null;
                AVClass? parentLogContext = null;
                AVClass** parentpp = default;
                if (ptr != null)
                {
                    avClass = **(AVClass**)ptr;
                    if (avClass.Value.parent_log_context_offset != 0)
                    {
                        parentpp = *(AVClass***)((byte*)ptr + avClass.Value.parent_log_context_offset);
                        if (parentpp != null && *parentpp != null)
                        {
                            parentLogContext = **parentpp;
                        }
                    }
                }

                var printPrefix = 1;
                var line = FFmpegCalls.FormatLine(ptr, level, Marshal.PtrToStringAnsi((nint)fmt) ?? string.Empty, vl, ref printPrefix);

                eventHandler(null,
                    new FFmpegLogReceivedEventArgs(avClass, parentLogContext, (LogLevel)level, line, ptr, parentpp));
            }
        }
    }

    internal static DirectoryInfo FindFfmpegDirectory(PlatformID platform)
    {
        var resolveEvent = ResolveFfmpegAssemblyLocation;
        var eventArgs = new ResolveFFmpegAssemblyLocationEventArgs(platform);
        if (resolveEvent != null)
        {
            resolveEvent(null, eventArgs);
        }

        return eventArgs.FFmpegDirectory;
    }
}
