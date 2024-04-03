using Rayer.Core.Abstractions;
using Rayer.Core.Models;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace Rayer.Services;

internal class ThemeResourceProvider : IThemeResourceProvider
{
    private static readonly string _themeResourceUriPrefix = "pack://application:,,,/Themes";
    private static readonly IDictionary<ApplicationTheme, Uri> _themeResource =
        new Dictionary<ApplicationTheme, Uri>()
        {
            [ApplicationTheme.Light] = new Uri($"{_themeResourceUriPrefix}/Symbols.xaml"),
            [ApplicationTheme.Dark] = new Uri($"{_themeResourceUriPrefix}/Symbols.Dark.xaml"),
        };

    private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

    public ThemeResourceProvider()
    {
        ApplicationThemeManager.Changed += ThemeChanged;
    }

    public PlaybarResource GetPlaybarResource()
    {
        var resources = Application.Current.Resources;

        return new PlaybarResource
        {
            Previous = (ImageSource)resources["Previous"],
            Next = (ImageSource)resources["Next"],
            Play = (ImageSource)resources["Play"],
            Pause = (ImageSource)resources["Pause"]
        };
    }

    private void ThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        _currentTheme = currentApplicationTheme;

        if (_currentTheme is ApplicationTheme.Light)
        {
            UpdateDictionary(_themeResource[ApplicationTheme.Light]);
        }
        else
        {
            UpdateDictionary(_themeResource[ApplicationTheme.Dark]);
        }
    }

    private static void UpdateDictionary(Uri newResourceUri)
    {
        var applicationDictionaries = Application.Current.Resources.MergedDictionaries;

        for (var i = 0; i < applicationDictionaries.Count; i++)
        {
            string sourceUri;

            if (applicationDictionaries[i]?.Source is not null)
            {
                sourceUri = applicationDictionaries[i].Source.ToString().ToLower().Trim();

                if (!sourceUri.Contains(';') && sourceUri.Contains("symbol"))
                {
                    applicationDictionaries[i] = new() { Source = newResourceUri };

                    return;
                }
            }

            for (var j = 0; j < applicationDictionaries[i].MergedDictionaries.Count; j++)
            {
                if (applicationDictionaries[i].MergedDictionaries[j]?.Source is null)
                {
                    continue;
                }

                sourceUri = applicationDictionaries[i]
                    .MergedDictionaries[j]
                    .Source.ToString().ToLower().Trim();

                if (sourceUri.Contains(';') || !sourceUri.Contains("symbol"))
                {
                    continue;
                }

                applicationDictionaries[i].MergedDictionaries[j] = new() { Source = newResourceUri };

                return;
            }
        }
    }
}