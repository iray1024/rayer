using NAudio.Extras;
using Rayer.Core.AudioEffect.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Utils;
using System.Collections.Immutable;
using System.IO;

namespace Rayer.Core.AudioEffect.Providers;

[Inject<IEqualizerProvider>]
internal class EqualizerProvider : IEqualizerProvider
{
    private readonly ISettingsService _settingsService;
    private static readonly IList<EqualizerBand[]> _sourceBands = [];
    private readonly ImmutableArray<EqualizerBand[]> _immutableSourceBands;

    public EqualizerProvider(ISettingsService settingsService)
    {
        _settingsService = settingsService;

        try
        {
            _immutableSourceBands = Initialize();

            ReplaceEqualizerBands(_sourceBands[Index]);
        }
        catch (DirectoryNotFoundException)
        {
            Available = false;
        }
        catch (FileNotFoundException)
        {
            Available = false;
        }
    }

    public EqualizerBand[] Equalizer { get; } = [
        new(){ Bandwidth = 0.8f, Frequency = 31, Gain = 0 },
        new(){ Bandwidth = 0.8f, Frequency = 62, Gain = 0 },
        new(){ Bandwidth = 0.8f, Frequency = 125, Gain = 0 },
        new(){ Bandwidth = 0.8f, Frequency = 250, Gain = 0 },
        new(){ Bandwidth = 0.8f, Frequency = 500, Gain = 0 },
        new(){ Bandwidth = 0.8f, Frequency = 1000, Gain = 0 },
        new(){ Bandwidth = 0.8f, Frequency = 2000, Gain = 0 },
        new(){ Bandwidth = 0.8f, Frequency = 4000, Gain = 0 },
        new(){ Bandwidth = 0.8f, Frequency = 8000, Gain = 0 },
        new(){ Bandwidth = 0.8f, Frequency = 16000, Gain = 0 }
    ];

    public bool Available { get; } = true;

    private int Index => (int)_settingsService.Settings.EqualizerMode;

    public void SwitchEqualizer(string identifier)
    {
        var currentMode = EnumHelper.ParseEnum<EqualizerMode>(identifier);
        var index = (int)currentMode;

        _settingsService.Settings.EqualizerMode = currentMode;
        _settingsService.Save();

        ReplaceEqualizerBands(_sourceBands[Index]);

        EqualizerChanged?.Invoke(null, EventArgs.Empty);
    }

    public void SwitchToCustom()
    {
        var oldModeIndex = (int)_settingsService.Settings.EqualizerMode;

        _settingsService.Settings.EqualizerMode = EqualizerMode.Custom;
        _settingsService.Save();

        Array.Copy(Equalizer, _sourceBands[11], IEqualizerProvider.EqualizerBandCount);
        ReplaceEqualizerBands(_sourceBands[Index]);
        Array.Copy(Clone(_immutableSourceBands[oldModeIndex]), _sourceBands[oldModeIndex], IEqualizerProvider.EqualizerBandCount);

        _settingsService.Settings.EqualizerMode = EqualizerMode.Custom;
        _settingsService.Save();

        EqualizerChanged?.Invoke(null, EventArgs.Empty);
    }

    public void SaveCustom()
    {
        Array.Copy(Equalizer, _sourceBands[11], IEqualizerProvider.EqualizerBandCount);

        var target = Path.Combine(Constants.Paths.AppDataDir, "equalizer", "自定义.json");

        Json<EqualizerBand[]>.StoreData(target, Equalizer);

    }

    public event EventHandler? EqualizerChanged;

    private static ImmutableArray<EqualizerBand[]> Initialize()
    {
        var sourceCopy = ImmutableArray<EqualizerBand[]>.Empty;

        #region 关闭均衡器        
        _sourceBands.Add([
            new(){ Bandwidth = 0.8f, Frequency = 31, Gain = 0 },
            new(){ Bandwidth = 0.8f, Frequency = 62, Gain = 0 },
            new(){ Bandwidth = 0.8f, Frequency = 125, Gain = 0 },
            new(){ Bandwidth = 0.8f, Frequency = 250, Gain = 0 },
            new(){ Bandwidth = 0.8f, Frequency = 500, Gain = 0 },
            new(){ Bandwidth = 0.8f, Frequency = 1000, Gain = 0 },
            new(){ Bandwidth = 0.8f, Frequency = 2000, Gain = 0 },
            new(){ Bandwidth = 0.8f, Frequency = 4000, Gain = 0 },
            new(){ Bandwidth = 0.8f, Frequency = 8000, Gain = 0 },
            new(){ Bandwidth = 0.8f, Frequency = 16000, Gain = 0 },
        ]);
        sourceCopy = sourceCopy.Add(Clone(_sourceBands[0]));
        #endregion

        var values = Enum.GetValues<EqualizerMode>();
        for (var i = 1; i < values.Length; i++)
        {
            var identifier = EnumHelper.GetDescription(values[i]);

            _sourceBands.Add(LoadEqualizer(identifier));
            sourceCopy = sourceCopy.Add(Clone(_sourceBands[i]));
        }

        return sourceCopy;
    }

    private static EqualizerBand[] LoadEqualizer(string identifier)
    {
        var root = Path.Combine(Constants.Paths.AppDataDir, "equalizer");

        var target = Path.Combine(root, $"{identifier}.json");

        return Json<EqualizerBand[]>.LoadData(target);
    }

    private void ReplaceEqualizerBands(EqualizerBand[] source)
    {
        Array.Copy(source, Equalizer, IEqualizerProvider.EqualizerBandCount);
    }

    private static EqualizerBand[] Clone(EqualizerBand[] source)
    {
        var newCopy = new EqualizerBand[10];
        for (var i = 0; i < source.Length; i++)
        {
            newCopy[i] = new EqualizerBand
            {
                Bandwidth = source[i].Bandwidth,
                Frequency = source[i].Frequency,
                Gain = source[i].Gain
            };
        }

        return newCopy;
    }
}