﻿using System.IO;
using System.Windows.Input;

namespace Rayer.Core;

public static class Constants
{
    public static class Paths
    {
        public static string AppDataDir { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rayer");

        public static string SettingsPath { get; } = Path.Combine(AppDataDir, "settings.json");
        public static string PlaylistPath { get; } = Path.Combine(AppDataDir, "playlist");
        public static string LyricPath { get; } = Path.Combine(AppDataDir, "lyric");
        public static string CookiePath { get; } = Path.Combine(AppDataDir, "cookie.dat");
        public static string BilibiliCookiePath { get; } = Path.Combine(AppDataDir, "bcookie.dat");
    }

    public static class DefaultKeyBinding
    {
        public static KeyBinding KeyPlayOrPause { get; } = new KeyBinding() { Key = Key.Space };
        public static KeyBinding KeyPrevious { get; } = new KeyBinding() { Key = Key.Z };
        public static KeyBinding KeyNext { get; } = new KeyBinding() { Key = Key.C };
        public static KeyBinding KeyPitchUp { get; } = new KeyBinding() { Key = Key.Up };
        public static KeyBinding KeyPitchDown { get; } = new KeyBinding() { Key = Key.Down };
        public static KeyBinding KeyForward { get; } = new KeyBinding() { Key = Key.Right };
        public static KeyBinding KeyRewind { get; } = new KeyBinding() { Key = Key.Left };
    }

    public static class SingleInstance
    {
#if DEBUG
        public const string UniqueAppName = "RAYER:MM-DEBUG";
#endif
#if RELEASE
        public const string UniqueAppName = "RAYER:MM";
#endif
        public static string PipeServerName { get; } = UniqueAppName + Environment.UserName;
    }
}