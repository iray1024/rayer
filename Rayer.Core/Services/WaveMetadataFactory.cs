using NAudio.Extras;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Rayer.Core.Abstractions;
using Rayer.Core.AudioEffect.Abstractions;
using Rayer.Core.AudioReader.Flac;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Http;
using Rayer.Core.Models;
using System.IO;

namespace Rayer.Core.Services;

[Inject<IWaveMetadataFactory>]
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

    async Task<WaveMetadata?> IWaveMetadataFactory.CreateAsync(string filepath)
    {
        Stream baseStream;
        WaveStream waveStream;
        var isWebStreaming = false;

        try
        {
            if (filepath.StartsWith("http"))
            {
                isWebStreaming = true;

                using var stream = await _httpClientProvider.HttpClient.GetStreamAsync(filepath, AppCore.StoppingToken);

                var buffer = new MemoryStream();

                await stream.CopyToAsync(buffer, AppCore.StoppingToken);
                buffer.Position = 0;

                baseStream = buffer;
                waveStream = new StreamMediaFoundationReader(buffer, _meidaSettings);
            }
            else
            {
                baseStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);

                var extension = Path.GetExtension(filepath);
#pragma warning disable IDE0045
                if (extension is ".flac")
                {
                    waveStream = new FlacAudioReader(baseStream);
                }
                else if (extension is ".ogg")
                {
                    waveStream = new VorbisWaveReader(baseStream, false);
                }
                else
                {
                    waveStream = new StreamMediaFoundationReader(baseStream, _meidaSettings);
                }
#pragma warning restore IDE0045
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
                FadeInOutSampleProvider = fadeInOutProvider,
                IsWebStreaming = isWebStreaming
            };
        }
        catch
        {
            return null;
        }
    }
}