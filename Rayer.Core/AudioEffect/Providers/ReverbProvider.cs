using NAudio.Wave;

namespace Rayer.Core.AudioEffect.Providers;

internal class ReverbProvider : ISampleProvider
{
    private readonly ISampleProvider _source;
    private readonly int _delayBufferLength;
    private readonly float[] _delayBuffer;
    private int _delayBufferPosition;
    private readonly float _decayFactor;

    private readonly float _roomSize;
    private readonly float _dampening;
    private readonly float _effectStrength;

    public ReverbProvider(ISampleProvider source, float decayTimeInSeconds, float roomSize, float dampening, float effectStrength)
    {
        _source = source;

        _delayBufferLength = (int)(decayTimeInSeconds * _source.WaveFormat.SampleRate);
        _delayBuffer = new float[_delayBufferLength];
        _delayBufferPosition = 0;
        _decayFactor = CalculateDecayFactor(decayTimeInSeconds);

        _roomSize = roomSize;
        _dampening = dampening;
        _effectStrength = effectStrength;
    }

    public WaveFormat WaveFormat => _source.WaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
        var samplesRead = _source.Read(buffer, offset, count);

        for (var i = 0; i < samplesRead; i++)
        {
            var inputSample = buffer[offset + i];

            var delayedSample = _delayBuffer[_delayBufferPosition];
            var reverbSample = _roomSize * _decayFactor * delayedSample;

            reverbSample *= 1.0f - _dampening;

            buffer[offset + i] += _effectStrength * reverbSample;

            _delayBuffer[_delayBufferPosition] = inputSample + (_effectStrength * reverbSample);

            _delayBufferPosition++;

            if (_delayBufferPosition >= _delayBufferLength)
            {
                _delayBufferPosition = 0;
            }
        }

        return samplesRead;
    }

    private float CalculateDecayFactor(float decayTimeInSeconds)
    {
        return (float)Math.Pow(10, -3.0 / (decayTimeInSeconds * _source.WaveFormat.SampleRate));
    }
}