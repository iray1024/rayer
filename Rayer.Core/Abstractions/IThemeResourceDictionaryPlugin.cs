using Wpf.Ui.Appearance;

namespace Rayer.Core.Abstractions;

public interface IThemeResourceDictionaryPlugin
{
    (string Namespace, IDictionary<ApplicationTheme, Uri> Resources) Register();
}