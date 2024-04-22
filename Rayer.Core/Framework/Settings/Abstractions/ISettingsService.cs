namespace Rayer.Core.Framework.Settings.Abstractions;

public interface ISettingsService
{
    ISettings Settings { get; }

    void Save();

    void Load();

    event EventHandler SettingsChanged;
}