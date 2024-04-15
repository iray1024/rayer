using Rayer.FFmpegCore.Modules.Abstractions;

namespace Rayer.FFmpegCore.Modules.Extensions;

internal static class IAudioSourceExtensions
{
    public static long GetRawElements(this IAudioSource source, TimeSpan timespan)
    {
        return source == null
            ? throw new ArgumentNullException(nameof(source))
            : TimeConverterFactory.Instance
                .GetTimeConverterForSource(source)
                .ToRawElements(source.WaveFormat, timespan);
    }

    public static long GetMilliseconds(this IAudioSource source, long elementCount)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegative(elementCount);

        return (long)GetTime(source, elementCount).TotalMilliseconds;
    }

    public static TimeSpan GetTime(this IAudioSource source, long elementCount)
    {
        ArgumentNullException.ThrowIfNull(source);

        return elementCount < 0
            ? throw new ArgumentNullException(nameof(elementCount))
            : TimeConverterFactory.Instance
                .GetTimeConverterForSource(source)
                .ToTimeSpan(source.WaveFormat, elementCount);
    }
}