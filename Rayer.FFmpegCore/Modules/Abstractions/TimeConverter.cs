namespace Rayer.FFmpegCore.Modules.Abstractions;

internal abstract class TimeConverter
{
    public static readonly TimeConverter SampleSourceTimeConverter = new SampleSourceTimeConverter_();

    public static readonly TimeConverter WaveSourceTimeConverter = new WaveSourceTimeConverter_();

    public abstract long ToRawElements(WaveFormat waveFormat, TimeSpan timeSpan);

    public abstract TimeSpan ToTimeSpan(WaveFormat waveFormat, long rawElements);

    internal class SampleSourceTimeConverter_ : TimeConverter
    {
        public override long ToRawElements(WaveFormat waveFormat, TimeSpan timeSpan)
            => waveFormat.MillisecondsToBytes(timeSpan.TotalMilliseconds) / waveFormat.BytesPerSample;

        public override TimeSpan ToTimeSpan(WaveFormat waveFormat, long rawElements)
            => TimeSpan.FromMilliseconds(waveFormat.BytesToMilliseconds(rawElements * waveFormat.BytesPerSample));
    }

    internal class WaveSourceTimeConverter_ : TimeConverter
    {
        public override long ToRawElements(WaveFormat waveFormat, TimeSpan timeSpan)
            => waveFormat.MillisecondsToBytes(timeSpan.TotalMilliseconds);

        public override TimeSpan ToTimeSpan(WaveFormat waveFormat, long rawElements)
            => TimeSpan.FromMilliseconds(waveFormat.BytesToMilliseconds(rawElements));
    }
}