using Rayer.Core.Abstractions;
using Rayer.Core.Framework.Injection;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace Rayer.Services;

[Inject<IThemeResourceProvider>]
internal class ThemeResourceProvider : IThemeResourceProvider
{
    private static readonly string _themeResourceUriPrefix = "pack://application:,,,/Themes";
    private static readonly IDictionary<ApplicationTheme, Uri> _themeResource =
        new Dictionary<ApplicationTheme, Uri>()
        {
            [ApplicationTheme.Light] = new Uri($"{_themeResourceUriPrefix}/Symbols.xaml"),
            [ApplicationTheme.Dark] = new Uri($"{_themeResourceUriPrefix}/Symbols.Dark.xaml"),
        };

    private readonly IEnumerable<(string Namespace, IDictionary<ApplicationTheme, Uri> Resources)> _pluginsThemeResources;

    private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

    public ThemeResourceProvider()
    {
        _pluginsThemeResources = App.GetServices<IThemeResourceDictionaryPlugin>().Select(x => x.Register());

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
            UpdateDictionary(string.Empty, _themeResource[ApplicationTheme.Light]);

            foreach (var (Namespace, Resources) in _pluginsThemeResources)
            {
                UpdateDictionary(Namespace, Resources[ApplicationTheme.Light]);
            }
        }
        else
        {
            UpdateDictionary(string.Empty, _themeResource[ApplicationTheme.Dark]);

            foreach (var (Namespace, Resources) in _pluginsThemeResources)
            {
                UpdateDictionary(Namespace, Resources[ApplicationTheme.Dark]);
            }
        }
    }

    private static void UpdateDictionary(string @namespace, Uri newResourceUri)
    {
        var applicationDictionaries = Application.Current.Resources.MergedDictionaries;

        for (var i = 0; i < applicationDictionaries.Count; i++)
        {
            string sourceUri;

            if (applicationDictionaries[i]?.Source is not null)
            {
                sourceUri = applicationDictionaries[i].Source.ToString().ToLower().Trim();

                if (sourceUri.Contains(@namespace, StringComparison.OrdinalIgnoreCase) && sourceUri.Contains("symbol"))
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

                if (!sourceUri.Contains("symbol") || !sourceUri.Contains(@namespace))
                {
                    continue;
                }

                applicationDictionaries[i].MergedDictionaries[j] = new() { Source = newResourceUri };

                return;
            }
        }
    }
}