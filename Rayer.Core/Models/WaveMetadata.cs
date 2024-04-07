using NAudio.Extras;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.IO;

namespace Rayer.Core.Models;

public class WaveMetadata : ISampleProvider, IDisposable
{
    public Stream? BaseStream { get; set; }

    public WaveStream? Reader { get; set; }

    public SmbPitchShiftingSampleProvider? PitchShiftingSampleProvider { get; set; }

    public FadeInOutSampleProvider? FadeInOutSampleProvider { get; set; }

    public Equalizer? Equalizer { get; set; }

    public WaveFormat WaveFormat => FadeInOutSampleProvider?.WaveFormat
        ?? throw new ObjectDisposedException("FadeInOutSampleProvider已被释放，无法读取WaveFormat");

    public int Read(float[] buffer, int offset, int count)
    {
        return FadeInOutSampleProvider is null
            ? throw new ObjectDisposedException("FadeInOutSampleProvider已被释放")
            : FadeInOutSampleProvider.Read(buffer, offset, count);
    }

    public static implicit operator FadeInOutSampleProvider(WaveMetadata metadata)
    {
        return metadata.FadeInOutSampleProvider
            ?? throw new NullReferenceException("当前Metadata的FadeInOutSampleProvider为空，无法隐式转换");
    }

    public void Dispose()
    {
        PitchShiftingSampleProvider = null;
        Equalizer = null;
        FadeInOutSampleProvider = null;

        Reader?.Close();
        Reader?.Dispose();
        Reader = null;

        BaseStream?.Close();
        BaseStream?.Dispose();
        BaseStream = null;

        GC.SuppressFinalize(this);
    }
}