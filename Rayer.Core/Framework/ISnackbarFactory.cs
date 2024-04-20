namespace Rayer.Core.Framework;

public interface ISnackbarFactory
{
    void ShowSecondary(string title, string message, TimeSpan? timeout = null);
}