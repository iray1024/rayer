using Microsoft.Win32;
using Rayer.Installer.Abstractions;
using Rayer.Installer.Models;
using System.IO;
using System.Windows.Media.Imaging;

namespace Rayer.Installer.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly ResourceMap _programResourceMap = null!;

    private BitmapImage _copyright = null!;
    private BitmapImage _wife = null!;
    private string _installPath = string.Empty;
    private bool _isInstallStart = false;
    private Progress<double> _progress = new();

    public MainViewModel()
    {
        _programResourceMap = Constants.Resources.First(n => n.Name == "Program");

        Copyright = new BitmapImage();

        Copyright.BeginInit();
        Copyright.UriSource = new Uri(Path.Combine(Constants.InstallerTempDir, "copyright.jpg"));
        Copyright.CacheOption = BitmapCacheOption.OnLoad;
        Copyright.EndInit();

        //Partner = new BitmapImage();

        //Partner.BeginInit();
        //Partner.UriSource = new Uri(Path.Combine(Constants.InstallerTempDir, "partner.jpg"));
        //Partner.CacheOption = BitmapCacheOption.OnLoad;
        //Partner.EndInit();

        using var key = Registry.CurrentUser.OpenSubKey(Constants.RegisterKey);

        InstallPath = key is not null
            ? key.GetValue("Install Path")?.ToString() ?? Constants.DefaultInstallPath
            : Constants.DefaultInstallPath;
    }

    public BitmapImage Copyright
    {
        get => _copyright;
        set
        {
            _copyright = value;
            OnPropertyChanged();
        }
    }

    public BitmapImage Partner
    {
        get => _wife;
        set
        {
            _wife = value;
            OnPropertyChanged();
        }
    }

    public string InstallPath
    {
        get => _installPath;
        set
        {
            _installPath = value;
            OnPropertyChanged();
        }
    }

    public bool IsInstallStart
    {
        get => _isInstallStart;
        set
        {
            _isInstallStart = value;
            _programResourceMap.DestinationDirectory = _installPath;
            OnPropertyChanged();
        }
    }

    public Progress<double> Progress
    {
        get => _progress;
        set
        {
            _progress ??= value;
            OnPropertyChanged();
        }
    }

    private RelayCommand _changeInstallPathCommand = null!;
    public RelayCommand ChangeInstallPathCommand
    {
        get
        {
            _changeInstallPathCommand ??= new RelayCommand(
                    param => ChangeInstallPath());
            return _changeInstallPathCommand;
        }
    }

    private void ChangeInstallPath()
    {
        var folderBrowserDialog = new FolderBrowserDialog
        {
            SelectedPath = InstallPath
        };

        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            if (string.Equals(folderBrowserDialog.SelectedPath, InstallPath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            InstallPath = folderBrowserDialog.SelectedPath;
        }
    }
}