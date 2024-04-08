using NAudio.Wave;
using Rayer.Core.Abstractions;
using SoundTouch.Net.NAudioSupport;

namespace Rayer.Core.Providers;

internal class SoundToucPitchShiftingProvider : IPitchShiftingProvider
{
    private readonly SoundTouchWaveProvider _soundTouchWaveProvider;

    public SoundToucPitchShiftingProvider(IWaveProvider waveProvider)
    {
        _soundTouchWaveProvider = new(waveProvider);
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