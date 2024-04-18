using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Abstractions;
using Rayer.SearchEngine.Controls;
using System.Windows;
using System.Windows.Controls;

namespace Rayer.SearchEngine.Services;

[Inject<ILoaderProvider>]
internal class LoaderProvider : ILoaderProvider
{
    private readonly Loader _loader = new();
    private int _offsetX = 0;
    private int _offsetY = 0;

    public Control Loader => _loader;

    private int _isLoading = 0;
    public bool IsLoading
    {
        get => _isLoading == 1;
        set => _ = Interlocked.Exchange(ref _isLoading, value ? 1 : 0);
    }

    public void SetLoader(ContentPresenter presenter, int offsetX = 0, int offsetY = 0)
    {
        presenter.Content = _loader;

        _offsetX = offsetX;
        _offsetY = offsetY;

        if (presenter.Parent is FrameworkElement e)
        {
            Loader.Width = e.ActualWidth + _offsetX;
            Loader.Height = e.ActualHeight + _offsetY;

            e.SizeChanged += OnSizeChanged;
        }

        Panel.SetZIndex(Loader, 99999);
    }

    public void Loading()
    {
        IsLoading = true;

        _loader.PART_Loader.Visibility = Visibility.Visible;
    }

    public void Loaded()
    {
        IsLoading = false;

        _loader.PART_Loader.Visibility = Visibility.Collapsed;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _loader.Width = e.NewSize.Width + _offsetX;
        _loader.Height = e.NewSize.Height + _offsetY;
    }
}