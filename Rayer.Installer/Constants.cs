using Rayer.Installer.Models;
using System.IO;

namespace Rayer.Installer;

internal static class Constants
{
    public static ResourceMap[] Resources =
    {
        new()
        {
            Name = "Program",
            Path = "Rayer.Installer.assets.rayer.zip",
            DestinationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Rayer")
        },
        new(){
            Name = "Equalizer",
            Path = "Rayer.Installer.assets.equalizer.zip",
            IsReplace = false,
            DestinationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rayer")
        },
        new(){
            Name = "CloudServer",
            Path = "Rayer.Installer.assets.cloud-server.zip",
            IsReplace = false,
            DestinationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rayer")
        },
        new(){
            Name = "Updater",
            Path = "Rayer.Installer.assets.updater.zip",
            IsReplace = false,
            DestinationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rayer")
        }
    };

    public static string DefaultInstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Rayer");

    public static string InstallerTempDir = Path.Combine(Path.GetTempPath(), "Rayer Installer");

    public const string RegisterKey = @"SOFTWARE\Rayer";
}