namespace Rayer.Core.Framework;

public interface IWindowsProviderService
{
    void Show<T>() where T : class;
}