﻿using Rayer.Core.Framework;
using Rayer.FrameworkCore;
using System.Windows;

namespace Rayer.Core.Controls;

public abstract class AdaptivePage(AdaptiveViewModelBase viewModel) : NoneFocusablePage
{
    protected AdaptiveViewModelBase ViewModel { get; set; } = viewModel;

    protected virtual void OnLoaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged += OnSizeChanged;

        var panelWidth = (AppCore.MainWindow.ActualWidth - 180 - ((int)ActualWidth >> 1)) / 3;

        ViewModel.TitleMaxWidth = panelWidth + 50;
        ViewModel.ArtistsNameMaxWidth = panelWidth + 50;
        ViewModel.AlbumNameMaxWidth = panelWidth + 80;

        ViewModel.DurationMaxWidth = e.Source is Window { WindowState: WindowState.Maximized } ? 43 : 39;
        ViewModel.ItemMargin = e.Source is Window { WindowState: WindowState.Maximized }
            ? new Thickness(0, 0, 30, 0)
            : new Thickness(0, 0, 24, 0);

        var orginalWidth = AppCore.MainWindow.Width;

        //AppCore.MainWindow.Width = orginalWidth + 1;
        //AppCore.MainWindow.Width = orginalWidth;
    }

    protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged -= OnSizeChanged;

        ViewModel = default!;
    }

    protected virtual void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var panelWidth = (e.NewSize.Width - 180 - ((int)ActualWidth >> 1)) / 3;

        ViewModel.TitleMaxWidth = panelWidth + 50;
        ViewModel.ArtistsNameMaxWidth = panelWidth + 50;
        ViewModel.AlbumNameMaxWidth = panelWidth + 80;

        ViewModel.DurationMaxWidth = e.Source is Window { WindowState: WindowState.Maximized } ? 43 : 39;
        ViewModel.ItemMargin = e.Source is Window { WindowState: WindowState.Maximized }
            ? new Thickness(0, 0, 30, 0)
            : new Thickness(0, 0, 24, 0);
    }
}