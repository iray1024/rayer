using NAudio.Wave;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Providers;

namespace Rayer.Core.Services;

internal class PitchShiftingProviderFactory : IPitchShiftingProviderFactory
{
    private readonly ISettingsService _settingsService;

    public PitchShiftingProviderFactory(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public IPitchShiftingProvider Create(WaveStream waveStream)
    {
        if (_settingsService.Settings.PitchProvider is PitchProvider.NAudio)
        {
            return new NAudioPitchShiftingProvider(waveStream.ToSampleProvider());
        }
        else if (_settingsService.Settings.PitchProvider is PitchProvider.SoundTouch)
        {
            return new SoundToucPitchShiftingProvider(waveStream);
        }

        throw new NotImplementedException();
    }
}