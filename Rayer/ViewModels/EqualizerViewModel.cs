using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NAudio.Extras;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using System.ComponentModel;
using System.Windows.Controls;

namespace Rayer.ViewModels;

public partial class EqualizerViewModel : ObservableObject
{
    private readonly IAudioManager _audioManager;
    private readonly IEqualizerProvider _equalizerProvider;
    private readonly ISettingsService _settingsService;

    private readonly EqualizerBand[] _bands;

    public EqualizerViewModel(
        IAudioManager audioManager,
        IEqualizerProvider equalizerProvider,
        ISettingsService settingsService)
    {
        _audioManager = audioManager;
        _equalizerProvider = equalizerProvider;
        _settingsService = settingsService;

        _bands = _equalizerProvider.Equalizer;

        _equalizerProvider.EqualizerChanged += OnEqualizerChanged;
    }

    public static float MinimumGain => -12;

    public static float MaximumGain => 12;

    #region Bands    
    public float Band1
    {
        get => _bands[0].Gain;
        set
        {
            var roundValue = MathF.Round(value, 2, MidpointRounding.ToZero);
            if (_bands[0].Gain != roundValue)
            {
                _bands[0].Gain = roundValue;
                OnPropertyChanged(nameof(Band1));
            }
        }
    }

    public float Band2
    {
        get => _bands[1].Gain;
        set
        {
            var roundValue = MathF.Round(value, 2, MidpointRounding.ToZero);
            if (_bands[1].Gain != roundValue)
            {
                _bands[1].Gain = roundValue;
                OnPropertyChanged(nameof(Band2));
            }
        }
    }

    public float Band3
    {
        get => _bands[2].Gain;
        set
        {
            var roundValue = MathF.Round(value, 2, MidpointRounding.ToZero);
            if (_bands[2].Gain != roundValue)
            {
                _bands[2].Gain = roundValue;
                OnPropertyChanged(nameof(Band3));
            }
        }
    }

    public float Band4
    {
        get => _bands[3].Gain;
        set
        {
            var roundValue = MathF.Round(value, 2, MidpointRounding.ToZero);
            if (_bands[3].Gain != roundValue)
            {
                _bands[3].Gain = roundValue;
                OnPropertyChanged(nameof(Band4));
            }
        }
    }

    public float Band5
    {
        get => _bands[4].Gain;
        set
        {
            var roundValue = MathF.Round(value, 2, MidpointRounding.ToZero);
            if (_bands[4].Gain != roundValue)
            {
                _bands[4].Gain = roundValue;
                OnPropertyChanged(nameof(Band5));
            }
        }
    }

    public float Band6
    {
        get => _bands[5].Gain;
        set
        {
            var roundValue = MathF.Round(value, 2, MidpointRounding.ToZero);
            if (_bands[5].Gain != roundValue)
            {
                _bands[5].Gain = roundValue;
                OnPropertyChanged(nameof(Band6));
            }
        }
    }

    public float Band7
    {
        get => _bands[6].Gain;
        set
        {
            var roundValue = MathF.Round(value, 2, MidpointRounding.ToZero);
            if (_bands[6].Gain != roundValue)
            {
                _bands[6].Gain = roundValue;
                OnPropertyChanged(nameof(Band7));
            }
        }
    }

    public float Band8
    {
        get => _bands[7].Gain;
        set
        {
            var roundValue = MathF.Round(value, 2, MidpointRounding.ToZero);
            if (_bands[7].Gain != roundValue)
            {
                _bands[7].Gain = roundValue;
                OnPropertyChanged(nameof(Band8));
            }
        }
    }

    public float Band9
    {
        get => _bands[8].Gain;
        set
        {
            var roundValue = MathF.Round(value, 2, MidpointRounding.ToZero);
            if (_bands[8].Gain != roundValue)
            {
                _bands[8].Gain = roundValue;
                OnPropertyChanged(nameof(Band9));
            }
        }
    }

    public float Band10
    {
        get => _bands[9].Gain;
        set
        {
            var roundValue = MathF.Round(value, 2, MidpointRounding.ToZero);
            if (_bands[9].Gain != roundValue)
            {
                _bands[9].Gain = roundValue;
                OnPropertyChanged(nameof(Band10));
            }
        }
    }
    #endregion

    public void SwitchToCustom()
    {
        _equalizerProvider.SwitchToCustom();
    }

    public void SaveCustom()
    {
        _equalizerProvider.SaveCustom();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        _audioManager.Playback.UpdateEqualizer();

        base.OnPropertyChanged(e);
    }

    [RelayCommand]
    private void EqualizerButtonClickCommand(object sender)
    {
        if (sender is RadioButton radio)
        {
            if (radio.Content is string identifier)
            {
                _equalizerProvider.SwitchEqualizer(identifier);
            }
        }
    }

    private void OnEqualizerChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(Band1));
        OnPropertyChanged(nameof(Band2));
        OnPropertyChanged(nameof(Band3));
        OnPropertyChanged(nameof(Band4));
        OnPropertyChanged(nameof(Band5));
        OnPropertyChanged(nameof(Band6));
        OnPropertyChanged(nameof(Band7));
        OnPropertyChanged(nameof(Band8));
        OnPropertyChanged(nameof(Band9));
        OnPropertyChanged(nameof(Band10));
    }
}