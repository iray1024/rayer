namespace Rayer.Core.AudioEffect.Abstractions;

internal interface IAudioEffect
{
    float Apply(float sample);
}