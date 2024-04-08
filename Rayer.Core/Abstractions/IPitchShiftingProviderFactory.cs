using NAudio.Wave;

namespace Rayer.Core.Abstractions;

internal interface IPitchShiftingProviderFactory
{
    IPitchShiftingProvider Create(WaveStream waveStream);
}