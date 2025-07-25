﻿using Rayer.Abstractions;
using Rayer.Controls.Immersive;
using Rayer.Core.Common;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.FrameworkCore.Injection;
using System.Windows;
using System.Windows.Controls;

namespace Rayer.Services;

[Inject<IImmersivePresenterProvider>]
internal class ImmersivePresenterProvider(ISettingsService settingsService) : IImmersivePresenterProvider
{
    private static readonly Lazy<ImmersiveVinylPresenter> _vinylPresenter;
    private static readonly Lazy<ImmersiveVisualizerPresenter> _audioVisualizerlPresenter;
    private static readonly Lazy<ImmersiveAlbumPresenter> _albumPresenter;

    static ImmersivePresenterProvider()
    {
        _vinylPresenter = new Lazy<ImmersiveVinylPresenter>(() => new ImmersiveVinylPresenter()
        {
            Margin = new Thickness(140, 180, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top
        });

        _audioVisualizerlPresenter = new Lazy<ImmersiveVisualizerPresenter>(() => new ImmersiveVisualizerPresenter()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        });

        _albumPresenter = new Lazy<ImmersiveAlbumPresenter>(() => new ImmersiveAlbumPresenter()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        });
    }

    public UserControl Presenter => settingsService.Settings.ImmersiveMode switch
    {
        ImmersiveMode.Vinyl => _vinylPresenter.Value,
        ImmersiveMode.AudioVisualizer => _audioVisualizerlPresenter.Value,
        ImmersiveMode.Album => _albumPresenter.Value,
        _ => throw new NotImplementedException()
    };
}