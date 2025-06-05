using IWshRuntimeLibrary;
using System.IO;

namespace Rayer.Installer.Services;

internal static class WindowsUtils
{
    public static void CreateShortcutOnDesktop(string name, string sourcePath, string description, string iconPath, string workingDirectory)
    {
        var targetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

        CreateShortcut(name + ".lnk", sourcePath, targetPath, description, iconPath, workingDirectory);
    }

    public static void AddToStartMenu(string name, string sourcePath, string description, string iconPath, string workingDirectory)
    {
        var targetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms));

        CreateShortcut(name + ".lnk", sourcePath, targetPath, description, iconPath, workingDirectory);
    }

    private static void CreateShortcut(string name, string sourcePath, string targetPath, string description, string iconPath, string workingDirectory)
    {
        var shortcutFilePath = Path.Combine(targetPath, name);

        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        if (!System.IO.File.Exists(shortcutFilePath))
        {
            CreateShortcutNew(name, sourcePath, targetPath, description, iconPath, "", "", workingDirectory);
        }
        else
        {
            ReCreateShortcutNew(name, sourcePath, targetPath, description, iconPath, "", "", workingDirectory);
        }
    }

    private static void CreateShortcutNew(string name, string sourcePath, string targetPath, string description, string iconPath, string commandLine, string hotKey, string workingDirectory)
    {
        var shortcutFilePath = Path.Combine(targetPath, name);

        var shortcut = CreateWshShortcut(shortcutFilePath);

        ArgumentNullException.ThrowIfNull(shortcut);

        shortcut.TargetPath = sourcePath;
        shortcut.Description = description;
        shortcut.WorkingDirectory = workingDirectory;
        shortcut.Arguments = commandLine;
        shortcut.IconLocation = iconPath;

        shortcut.Save();
    }

    private static void ReCreateShortcutNew(string name, string sourcePath, string targetPath, string description, string iconPath, string commandLine, string hotKey, string workingDirectory)
    {
        var oldShortcutFilePath = Path.Combine(targetPath, name);

        var shortcut = CreateWshShortcut(oldShortcutFilePath);

        ArgumentNullException.ThrowIfNull(shortcut);

        var oldPath = shortcut.TargetPath;
        if (oldPath.Equals(sourcePath))
        {
            return;
        }
        else
        {
            System.IO.File.Delete(oldShortcutFilePath);

            CreateShortcutNew(name, sourcePath, targetPath, description, iconPath, commandLine, hotKey, workingDirectory);
        }
    }

    private static IWshShortcut? CreateWshShortcut(string shortcutFilePath)
        => new WshShell().CreateShortcut(shortcutFilePath) as IWshShortcut;
}