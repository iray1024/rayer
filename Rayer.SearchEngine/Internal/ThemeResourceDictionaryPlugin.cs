using Rayer.Core.Abstractions;
using Rayer.Core.Framework.Injection;
using Wpf.Ui.Appearance;

namespace Rayer.SearchEngine.Internal;

[Inject<IThemeResourceDictionaryPlugin>]
internal class ThemeResourceDictionaryPlugin : IThemeResourceDictionaryPlugin
{
    private static readonly string _themeResourceUriPrefix = "pack://application:,,,/Rayer.SearchEngine;component/Themes";

    public (string Namespace, IDictionary<ApplicationTheme, Uri> Resources) Register()
    {
        return ("Rayer.SearchEngine", new Dictionary<ApplicationTheme, Uri>()
        {
            [ApplicationTheme.Light] = new Uri($"{_themeResourceUriPrefix}/Symbols.xaml"),
            [ApplicationTheme.Dark] = new Uri($"{_themeResourceUriPrefix}/Symbols.Dark.xaml"),
        });
    }
}