namespace Rayer.FFmpegCore.Modules.Abstractions;

internal interface IAudioSource : IDisposable
{
    bool CanSeek { get; }

    WaveFormat WaveFormat { get; }

    long Position { get; set; }

    long Length { get; }
}