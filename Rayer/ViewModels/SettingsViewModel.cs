using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Rayer.ViewModels;

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

    public SettingsViewModel(
        INavigationService navigationService,
        ISettingsService settings)
    {
        _navigationService = navigationService;
        _settings = settings;

        _audioLibrary = _settings.Settings.AudioLibrary;
        _currentApplicationTheme = _settings.Settings.Theme;
        _playSingleAudioStrategy = _settings.Settings.PlaySingleAudioStrategy;
        _immersiveMode = _settings.Settings.ImmersiveMode;

        AudioLibrary.CollectionChanged += AudioLibrary_CollectionChanged;
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
        ApplicationThemeManager.Apply(newValue, WindowBackdropType.Mica, true, true);
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

    private void AudioLibrary_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(IsEmpty));
    }
}