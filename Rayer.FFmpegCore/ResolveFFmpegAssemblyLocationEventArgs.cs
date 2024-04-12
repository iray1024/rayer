namespace Rayer.FFmpegCore;

/// <summary>
/// Provides data for the <see cref="FFmpegUtils.ResolveFfmpegAssemblyLocation"/> event.
/// </summary>
public class ResolveFFmpegAssemblyLocationEventArgs : EventArgs
{
    internal ResolveFFmpegAssemblyLocationEventArgs(PlatformID platformId)
    {
        Platform = platformId;
    }

    /// <summary>
    /// Gets the platform.
    /// </summary>
    /// <value>
    /// The platform.
    /// </value>
    public PlatformID Platform { get; private set; }

    /// <summary>
    /// Gets or sets the directory which contains the native Ffmpeg assemblies for the current <see cref="Platform"/> and architecture.
    /// </summary>
    /// <value>
    /// The FFmpeg directory.
    /// </value>
    public DirectoryInfo FFmpegDirectory { get; set; } = null!;
}