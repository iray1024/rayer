using NAudio.Wave;

namespace Rayer.Core.AudioEffect.Providers;

internal class EchoSampleProvider : ISampleProvider
{
    private readonly ISampleProvider _source;

    private readonly int _echoDelayInSamples;
    private readonly float _echoGain;
    private readonly float _decay;
    private readonly float[] _delayBuffer;

    private int position;

    public EchoSampleProvider(ISampleProvider source, int echoDelayInMilliseconds, float echoGain, float decay)
    {
        _source = source;

        _echoDelayInSamples = (int)(source.WaveFormat.SampleRate * echoDelayInMilliseconds / 1000.0);
        _echoGain = echoGain;
        _decay = decay;
        _delayBuffer = new float[_echoDelayInSamples];
    }

    public WaveFormat WaveFormat => _source.WaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
        var samplesRead = _source.Read(buffer, offset, count);

        for (var i = 0; i < samplesRead; i++)
        {
            var inputSample = buffer[offset + i];

            var echoSample = _delayBuffer[position];

            _delayBuffer[position] = inputSample + (echoSample * _echoGain);
            buffer[offset + i] += echoSample;

            _delayBuffer[position] *= _decay;

            position++;

            if (position >= _echoDelayInSamples)
            {
                position = 0;
            }
        }

        return samplesRead;
    }
}