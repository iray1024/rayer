using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Rayer.Core.Abstractions;

namespace Rayer.Core.Providers;

internal class NAudioPitchShiftingProvider : IPitchShiftingProvider
{
    private readonly SmbPitchShiftingSampleProvider _smbPitchShiftingSampleProvider;

    public NAudioPitchShiftingProvider(ISampleProvider sampleProvider)
    {
        _smbPitchShiftingSampleProvider = new(sampleProvider);
    }

    public float Pitch
    {
        get => _smbPitchShiftingSampleProvider.PitchFactor;
        set => _smbPitchShiftingSampleProvider.PitchFactor = value;
    }

    public float PitchOctaves
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public float PitchSemiTones
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public ISampleProvider ToSampleProvider()
    {
        return _smbPitchShiftingSampleProvider;
    }
}