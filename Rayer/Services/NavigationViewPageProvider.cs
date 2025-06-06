using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using Wpf.Ui.Abstractions;

namespace Rayer.Services;

[Inject<INavigationViewPageProvider>]
internal sealed class NavigationViewPageProvider : INavigationViewPageProvider
{
    public object? GetPage(Type pageType)
    {
        return AppCore.GetService(pageType);
    }
}