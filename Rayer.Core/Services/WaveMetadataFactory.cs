using NAudio.Extras;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Rayer.Core.Abstractions;
using Rayer.Core.Models;
using System.IO;

namespace Rayer.Core.Services;

internal class WaveMetadataFactory : IWaveMetadataFactory
{
    private readonly MediaFoundationReader.MediaFoundationReaderSettings _meidaSettings = new() { RequestFloatOutput = true };

    private readonly IEqualizerProvider _equalizerProvider;
    private readonly IPitchShiftingProviderFactory _pitchShiftingProviderFactory;

    public WaveMetadataFactory(
        IEqualizerProvider equalizerProvider,
        IPitchShiftingProviderFactory pitchShiftingProviderFactory)
    {
        _equalizerProvider = equalizerProvider;
        _pitchShiftingProviderFactory = pitchShiftingProviderFactory;
    }

    WaveMetadata IWaveMetadataFactory.CreateWaveMetadata(string filepath)
    {
        var baseStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
        
        WaveStream waveStream;
        if (Path.GetExtension(filepath) is ".ogg")
        {
            waveStream = new VorbisWaveReader(baseStream, false);
        }
        else
        {
            waveStream = new StreamMediaFoundationReader(baseStream, _meidaSettings);
        }

        var pitchProvider = _pitchShiftingProviderFactory.Create(waveStream);

        var equalizer = new Equalizer(pitchProvider.ToSampleProvider(), _equalizerProvider.Equalizer);

        var fadeInOutProvider = new FadeInOutSampleProvider(equalizer);
        
        return new WaveMetadata
        {
            BaseStream = baseStream,
            Reader = waveStream,
            PitchShiftingSampleProvider = pitchProvider,
            Equalizer = equalizer,
            FadeInOutSampleProvider = fadeInOutProvider
        };
    }
}