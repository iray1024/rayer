using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;
using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using Rayer.Core.FileSystem.Abstractions;
using Rayer.Core.Framework;
using Rayer.Core.Framework.Injection;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Core.Options;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Rayer.ViewModels;

[Inject]
public sealed partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized = false;

    private readonly INavigationService _navigationService;
    private readonly ISettingsService _settings;

    [ObservableProperty]
    private string _appVersion = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _audioLibrary;

    public bool IsEmpty => AudioLibrary.Count == 0;

    private ApplicationTheme _currentApplicationTheme;
    public ApplicationTheme CurrentApplicationTheme
    {
        get => _currentApplicationTheme;
        set
        {
            SetProperty(ref _currentApplicationTheme, value);
            _settings.Settings.Theme = _currentApplicationTheme;
            OnCurrentApplicationThemeChanged(_currentApplicationTheme);
            OnPropertyChanged();
            UpdateConfigFile();
        }
    }

    private PlaySingleAudioStrategy _playSingleAudioStrategy;
    public PlaySingleAudioStrategy PlaySingleAudioStrategy
    {
        get => _playSingleAudioStrategy;
        set
        {
            SetProperty(ref _playSingleAudioStrategy, value);
            _settings.Settings.PlaySingleAudioStrategy = _playSingleAudioStrategy;
            OnPropertyChanged();
            UpdateConfigFile();
        }
    }

    private ImmersiveMode _immersiveMode;
    public ImmersiveMode ImmersiveMode
    {
        get => _immersiveMode;
        set
        {
            SetProperty(ref _immersiveMode, value);
            _settings.Settings.ImmersiveMode = _immersiveMode;
            OnPropertyChanged();
            UpdateConfigFile();
        }
    }

    private PitchProvider _pitchProvider;
    public PitchProvider PitchProvider
    {
        get => _pitchProvider;
        set
        {
            SetProperty(ref _pitchProvider, value);
            _settings.Settings.PitchProvider = _pitchProvider;
            OnPropertyChanged();
            UpdateConfigFile();
            Task.Run(async () =>
            {
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    var deviceManager = App.GetRequiredService<IDeviceManager>();
                    await deviceManager.SwitchPitchProvider();
                });
            });
        }
    }

    private LyricSearcher _lyricSearcher;
    public LyricSearcher LyricSearcher
    {
        get => _lyricSearcher;
        set
        {
            SetProperty(ref _lyricSearcher, value);
            _settings.Settings.LyricSearcher = _lyricSearcher;
            OnPropertyChanged();
            UpdateConfigFile();
            Task.Run(async () =>
            {
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    if (AppCore.GetRequiredService<IAudioManager>().Playback.Playing)
                    {
                        var provider = App.GetRequiredService<ILyricProvider>();
                        await provider.SwitchSearcherAsync();
                    }
                });
            });
        }
    }

    private SearcherType _defaultSearcher;
    public SearcherType DefaultSearcher
    {
        get => _defaultSearcher;
        set
        {
            SetProperty(ref _defaultSearcher, value);
            _settings.Settings.DefaultSearcher = _defaultSearcher;
            AppCore.GetRequiredService<IOptionsSnapshot<SearchEngineOptions>>().Value.SearcherType = _defaultSearcher;
            OnPropertyChanged();
            UpdateConfigFile();
        }
    }

    [ObservableProperty]
    private bool _isCloudServerAvaliable = true;

    [ObservableProperty]
    private string _cloudServerPortNumber = "3000";

    public SettingsViewModel(
        INavigationService navigationService,
        ISettingsService settings,
        IIPSBootloader bootloader)
    {
        _navigationService = navigationService;
        _settings = settings;

        AudioLibrary = _settings.Settings.AudioLibrary;
        CurrentApplicationTheme = _settings.Settings.Theme;
        PlaySingleAudioStrategy = _settings.Settings.PlaySingleAudioStrategy;
        ImmersiveMode = _settings.Settings.ImmersiveMode;
        _pitchProvider = _settings.Settings.PitchProvider;
        _lyricSearcher = _settings.Settings.LyricSearcher;
        DefaultSearcher = _settings.Settings.DefaultSearcher;

        AudioLibrary.CollectionChanged += OnCollectionChanged;

        bootloader.Exited += OnBootloaderExited;

        IsCloudServerAvaliable = bootloader.IsServerAvaliable;
        CloudServerPortNumber = bootloader.Port != -1
            ? bootloader.Port.ToString()
            : "N/A";

        _settings.SettingsChanged += OnSettingsChanged;
    }

    private void OnSettingsChanged(object? sender, EventArgs e)
    {
        _immersiveMode = _settings.Settings.ImmersiveMode;
        _lyricSearcher = _settings.Settings.LyricSearcher;

        OnPropertyChanged(nameof(ImmersiveMode));
        OnPropertyChanged(nameof(LyricSearcher));
    }

    public void UpdateConfigFile()
    {
        _settings.Save();
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
        {
            InitializeViewModel();
        }
    }

    public void OnNavigatedFrom()
    {

    }

    private static void OnCurrentApplicationThemeChanged(ApplicationTheme newValue)
    {
        ApplicationThemeManager.Apply(newValue, WindowBackdropType.Mica, true);
    }

    private void InitializeViewModel()
    {
        CurrentApplicationTheme = ApplicationThemeManager.GetAppTheme();
        AppVersion = $"{GetAssemblyVersion()}";

        ApplicationThemeManager.Changed += OnThemeChanged;

        _isInitialized = true;
    }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        if (CurrentApplicationTheme != currentApplicationTheme)
        {
            CurrentApplicationTheme = currentApplicationTheme;
        }
    }

    private static string GetAssemblyVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
    }

    private RelayCommand _addLibraryCommand = default!;
    public RelayCommand AddLibraryCommand
    {
        get
        {
            _addLibraryCommand ??= new RelayCommand(param => AddLibrary());

            return _addLibraryCommand;
        }
    }

    private RelayCommand _removeLibraryCommand = default!;
    public RelayCommand RemoveLibraryCommand
    {
        get
        {
            _removeLibraryCommand ??= new RelayCommand(RemoveLibrary);

            return _removeLibraryCommand;
        }
    }

    [CommunityToolkit.Mvvm.Input.RelayCommand]
    private async Task RestartServer()
    {
        var snackbar = App.GetRequiredService<ISnackbarFactory>();
        var bootloader = App.GetRequiredService<IIPSBootloader>();

        var uri = await bootloader.Restart();

        var searchEngineOptions = App.GetRequiredService<IOptionsSnapshot<SearchEngineOptions>>().Value;

        searchEngineOptions.HttpEndpoint = uri.OriginalString;

        snackbar.ShowSecondary(
            "Cloud Server",
            $"Cloud Server重启成功",
            TimeSpan.FromSeconds(3));

        IsCloudServerAvaliable = true;
        CloudServerPortNumber = uri.Port.ToString();
    }

    private void AddLibrary()
    {
        var folderBrowserDialog = new FolderBrowserDialog()
        {
            RootFolder = Environment.SpecialFolder.MyMusic,
            AutoUpgradeEnabled = true,
            Description = "选择文件夹导入...",
            UseDescriptionForTitle = true,
        };

        if (folderBrowserDialog.ShowDialog() is DialogResult.OK)
        {
            _settings.Settings.AudioLibrary.Add(folderBrowserDialog.SelectedPath);
            UpdateConfigFile();

            var watcherService = App.GetRequiredService<IAudioFileWatcher>();
            watcherService.AddWatcher(folderBrowserDialog.SelectedPath);
        }
    }

    private void RemoveLibrary(object? param)
    {
        if (param is string path)
        {
            var watcherService = App.GetRequiredService<IAudioFileWatcher>();
            watcherService.RemoveWatcher(path);

            _settings.Settings.AudioLibrary.Remove(path);
            UpdateConfigFile();
        }
    }

    private void OnCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(IsEmpty));
    }

    private void OnBootloaderExited(object? sender, EventArgs e)
    {
        IsCloudServerAvaliable = false;
        CloudServerPortNumber = "N/A";
    }
}