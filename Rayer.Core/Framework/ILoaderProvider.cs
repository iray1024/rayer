using System.Windows.Controls;

namespace Rayer.Core.Framework;

public interface ILoaderProvider
{
    Control Loader { get; }

    bool IsLoading { get; }

    void SetLoader(ContentPresenter presenter, int offsetX = 0, int offsetY = 0);

    void Loading();

    void Loaded();
}