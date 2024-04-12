using NAudio.Wave;

namespace Rayer.Core.AudioEffect.Abstractions;

public interface IPitchShiftingProvider
{
    float Pitch { get; set; }

    float PitchOctaves { get; set; }

    float PitchSemiTones { get; set; }

    ISampleProvider ToSampleProvider();
}