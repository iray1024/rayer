using System.IO;

namespace Rayer.Core;

public static class Constants
{
    public static class Paths
    {
        public static string AppDataDir { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rayer");

        public static string SettingsPath { get; } = Path.Combine(AppDataDir, "settings.json");
    }
}