namespace Rayer.FFmpegCore.Modules.Abstractions;

internal interface IReadableAudioSource<in T> : IAudioSource
{
    int Read(T[] buffer, int offset, int count);
}