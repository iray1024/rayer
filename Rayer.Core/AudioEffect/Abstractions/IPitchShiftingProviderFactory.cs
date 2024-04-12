using NAudio.Wave;

namespace Rayer.Core.AudioEffect.Abstractions;

internal interface IPitchShiftingProviderFactory
{
    IPitchShiftingProvider Create(WaveStream waveStream);
}