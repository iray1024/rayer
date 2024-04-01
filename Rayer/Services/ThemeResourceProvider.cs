using Rayer.Core.Abstractions;
using Rayer.Core.Models;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace Rayer.Services;

internal class ThemeResourceProvider : IThemeResourceProvider
{
    private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

    public ThemeResourceProvider()
    {
        ApplicationThemeManager.Changed += ThemeChanged;
    }

    public PlaybarResource GetPlaybarResource()
    {
        var resources = Application.Current.Resources;

        return _currentTheme is ApplicationTheme.Light
            ? new PlaybarResource
            {
                Previous = (ImageSource)resources["PreviousLight"],
                Next = (ImageSource)resources["NextLight"],
                Play = (ImageSource)resources["PlayLight"],
                Pause = (ImageSource)resources["PauseLight"]
            }
            : new PlaybarResource
            {
                Previous = (ImageSource)resources["PreviousDark"],
                Next = (ImageSource)resources["NextDark"],
                Play = (ImageSource)resources["PlayDark"],
                Pause = (ImageSource)resources["PauseDark"]
            };
    }

    private void ThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        _currentTheme = currentApplicationTheme;
    }
}