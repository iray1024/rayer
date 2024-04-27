using NAudio.Dsp;
using NAudio.Wave;

namespace Rayer.Core.AudioEffect.Providers;

internal class HighPassFilterSampleProvider(ISampleProvider source, float filterCutoff) : ISampleProvider
{
    private readonly BiQuadFilter lowPassFilter = BiQuadFilter.HighPassFilter(source.WaveFormat.SampleRate, filterCutoff, 0.7071f);

    public WaveFormat WaveFormat => source.WaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
        var samplesRead = source.Read(buffer, offset, count);

        for (var i = 0; i < samplesRead; i++)
        {
            buffer[offset + i] = lowPassFilter.Transform(buffer[offset + i]);
        }

        return samplesRead;
    }
}