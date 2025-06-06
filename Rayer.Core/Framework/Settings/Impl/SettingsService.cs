using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Utils;
using Rayer.FrameworkCore.Injection;

namespace Rayer.Core.Framework.Settings.Impl;

[Inject<ISettingsService>]
internal class SettingsService : ISettingsService
{
    private readonly string _settingsPath = Constants.Paths.SettingsPath;

    public SettingsService()
    {
        Load();
    }

    public ISettings Settings { get; private set; } = null!;

    public event EventHandler? SettingsChanged;

    public void Save()
    {
        Json<ISettings>.StoreData(_settingsPath, Settings);

        SettingsChanged?.Invoke(this, EventArgs.Empty);
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