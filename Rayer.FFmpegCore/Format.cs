using Rayer.FFmpegCore.Interops;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Rayer.FFmpegCore;

/// <summary>
/// Represents a FFmpeg format.
/// </summary>
public class Format
{
    /*
     * In order to avoid duplicate code, we could use dynamic parameters ...
     * Unfortunately they are not supported by mono on .net 3.5
     */
    internal unsafe Format(AVOutputFormat format)
    {
        LongName = Marshal.PtrToStringAnsi((nint)format.long_name) ?? string.Empty;
        Name = Marshal.PtrToStringAnsi((nint)format.name) ?? string.Empty;

        Codecs = FFmpegCalls.GetCodecOfCodecTag(format.codec_tag).AsReadOnly();

        var extensions = Marshal.PtrToStringAnsi((nint)format.extensions);
        FileExtensions = !string.IsNullOrEmpty(extensions)
            ? extensions.Split(',').ToList().AsReadOnly()
            : Enumerable.Empty<string>().ToList().AsReadOnly();
    }

    internal unsafe Format(AVInputFormat format)
    {
        LongName = Marshal.PtrToStringAnsi((nint)format.long_name) ?? string.Empty;
        Name = Marshal.PtrToStringAnsi((nint)format.name) ?? string.Empty;

        Codecs = FFmpegCalls.GetCodecOfCodecTag(format.codec_tag).AsReadOnly();

        var extensions = Marshal.PtrToStringAnsi((nint)format.extensions);
        FileExtensions = !string.IsNullOrEmpty(extensions)
            ? extensions.Split(',').ToList().AsReadOnly()
            : Enumerable.Empty<string>().ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets the name of the format.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the long name of the format.
    /// </summary>
    public string LongName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a list of the common codecs.
    /// </summary>
    public ReadOnlyCollection<AvCodecId> Codecs { get; private set; }

    /// <summary>
    /// Gets a list with the common file extensions of the format.
    /// </summary>
    public ReadOnlyCollection<string> FileExtensions { get; private set; }
}