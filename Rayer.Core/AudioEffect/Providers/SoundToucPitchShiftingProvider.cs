using NAudio.Wave;
using Rayer.Core.AudioEffect.Abstractions;
using SoundTouch.Net.NAudioSupport;

namespace Rayer.Core.AudioEffect.Providers;

internal class SoundToucPitchShiftingProvider : IPitchShiftingProvider
{
    private readonly SoundTouchWaveProvider _soundTouchWaveProvider;

    public SoundToucPitchShiftingProvider(IWaveProvider waveProvider)
    {
        var waveformat = waveProvider.WaveFormat;

        var tempSampleProvicer = waveProvider
            .ToSampleProvider()
            .ToWaveProvider();

        _soundTouchWaveProvider = new(tempSampleProvicer, new SoundTouch.SoundTouchProcessor
        {
            Channels = waveformat.Channels,
            SampleRate = waveformat.SampleRate
        });
    }

    public float Pitch
    {
        get => (float)_soundTouchWaveProvider.Pitch;
        set => _soundTouchWaveProvider.Pitch = value;
    }

    public float PitchOctaves
    {
        get => (float)_soundTouchWaveProvider.PitchOctaves;
        set => _soundTouchWaveProvider.PitchOctaves = value;
    }

    public float PitchSemiTones
    {
        get => (float)_soundTouchWaveProvider.PitchSemiTones;
        set => _soundTouchWaveProvider.PitchSemiTones = value;
    }

    public ISampleProvider ToSampleProvider()
    {
        return _soundTouchWaveProvider.ToSampleProvider();
    }
}