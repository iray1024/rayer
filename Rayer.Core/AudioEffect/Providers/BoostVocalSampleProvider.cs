using NAudio.Dsp;
using NAudio.Wave;

namespace Rayer.Core.AudioEffect.Providers;

internal class BoostVocalSampleProvider(ISampleProvider source, float boostFactor, float centerFrequency) : ISampleProvider
{
    private readonly BiQuadFilter[] _filters =
        [
            BiQuadFilter.PeakingEQ(source.WaveFormat.SampleRate, centerFrequency, 2.0f, boostFactor)
        ];

    public WaveFormat WaveFormat => source.WaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
        var samplesRead = source.Read(buffer, offset, count);

        for (var i = 0; i < samplesRead; i++)
        {
            foreach (var filter in _filters)
            {
                buffer[offset + i] = filter.Transform(buffer[offset + i]);
            }
        }

        return samplesRead;
    }
}