using System.Windows;
using System.Windows.Markup;

namespace Rayer.Markup;

[ContentProperty(nameof(ResourceKey))]
[MarkupExtensionReturnType(typeof(object))]
public class ThemeSymbolExtension : DynamicResourceExtension
{
    public ThemeSymbolExtension()
    {
        ResourceKey = ThemeSymbol.Unknown.ToString();
    }

    public ThemeSymbolExtension(ThemeSymbol resourceKey)
    {
        if (resourceKey == ThemeSymbol.Unknown)
        {
            throw new ArgumentNullException(nameof(resourceKey));
        }

        ResourceKey = resourceKey.ToString();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Application.Current.Resources[ResourceKey];
    }
}