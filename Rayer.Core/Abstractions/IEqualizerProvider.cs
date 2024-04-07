using NAudio.Extras;

namespace Rayer.Core.Abstractions;

public interface IEqualizerProvider
{
    public const int EqualizerBandCount = 10;

    EqualizerBand[] Equalizer { get; }

    void SwitchEqualizer(string identifier);

    void SwitchToCustom();

    void SaveCustom();

    event EventHandler EqualizerChanged;
}