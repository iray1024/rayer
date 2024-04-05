using Rayer.Abstractions;
using Rayer.Controls.Immersive;
using Rayer.Core.Abstractions;
using Rayer.Core.Common;
using System.Windows;
using System.Windows.Controls;

namespace Rayer.Services;

internal class ImmersivePresenterProvider(ISettingsService settingsService) : IImmersivePresenterProvider
{
    private static readonly Lazy<ImmersiveVinylPresenter> _vinylPresenter;
    private static readonly Lazy<ImmersiveVisualizerPresenter> _audioVisualizerlPresenter;

    static ImmersivePresenterProvider()
    {
        _vinylPresenter = new Lazy<ImmersiveVinylPresenter>(() => new ImmersiveVinylPresenter()
        {
            Margin = new Thickness(140, -140, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        });
        _audioVisualizerlPresenter = new Lazy<ImmersiveVisualizerPresenter>(() => new ImmersiveVisualizerPresenter()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        });
    }

    public UserControl Presenter
    {
        get
        {
            return settingsService.Settings.ImmersiveMode is ImmersiveMode.Vinyl
                ? _vinylPresenter.Value
                : _audioVisualizerlPresenter.Value;
        }
    }
}