using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Utils;

namespace Rayer.Core.Framework.Settings.Impl;

internal class SettingsService : ISettingsService
{
    private readonly string _settingsPath = Constants.Paths.SettingsPath;

    public SettingsService()
    {
        Load();
    }

    public ISettings Settings { get; private set; } = null!;

    public void Save()
    {
        Json<ISettings>.StoreData(_settingsPath, Settings);
    }

    public void Load()
    {
        try
        {
            Settings = Json<Settings>.LoadData(_settingsPath);
        }
        catch
        {
            Settings = new Settings();
            Save();
        }
    }
}