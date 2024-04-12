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

    private readonly IHttpClientProvider _httpClientProvider;

    public WaveMetadataFactory(
        IEqualizerProvider equalizerProvider,
        IPitchShiftingProviderFactory pitchShiftingProviderFactory,
        IHttpClientProvider httpClientProvider)
    {
        _equalizerProvider = equalizerProvider;
        _pitchShiftingProviderFactory = pitchShiftingProviderFactory;
        _httpClientProvider = httpClientProvider;
    }

    WaveMetadata IWaveMetadataFactory.Create(string filepath)
    {
        Stream baseStream;
        WaveStream waveStream;

        if (filepath.StartsWith("http"))
        {
            var stream = _httpClientProvider.HttpClient.GetStreamAsync(filepath).Result;

            var buffer = new MemoryStream();

            stream.CopyTo(buffer);
            buffer.Position = 0;

            baseStream = buffer;
            waveStream = new StreamMediaFoundationReader(buffer, _meidaSettings);
        }
        else
        {
            baseStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);

            waveStream = Path.GetExtension(filepath) is ".ogg"
                ? new VorbisWaveReader(baseStream, false)
                : new StreamMediaFoundationReader(baseStream, _meidaSettings);
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