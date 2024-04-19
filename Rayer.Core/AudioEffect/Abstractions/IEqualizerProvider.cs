using NAudio.Extras;

namespace Rayer.Core.AudioEffect.Abstractions;

public interface IEqualizerProvider
{
    public const int EqualizerBandCount = 10;

    EqualizerBand[] Equalizer { get; }

    bool Available { get; }

    void SwitchEqualizer(string identifier);

    void SwitchToCustom();

    void SaveCustom();

    event EventHandler EqualizerChanged;
}