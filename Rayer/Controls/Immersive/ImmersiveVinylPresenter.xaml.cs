﻿using NAudio.Wave;
using Rayer.Core.Abstractions;
using Rayer.Core.Events;
using Rayer.FrameworkCore;
using Rayer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Rayer.Controls.Immersive;

public partial class ImmersiveVinylPresenter : UserControl
{
    private readonly IAudioManager _audioManager;

    private double _transformFactor = 160;

    public ImmersiveVinylPresenter()
    {
        var vm = App.GetRequiredService<ImmersiveVinylPresenterViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();

        AlbumOpenStoryboard = Resources["AlbumOpenStoryboard"] as Storyboard
            ?? throw new ArgumentNullException("未找到 AlbumOpenStoryboard 资源");

        AlbumRotateStoryboard = Resources["AlbumRotateStoryboard"] as Storyboard
            ?? throw new ArgumentNullException("未找到 AlbumRotateStoryboard 资源");

        AlbumBoxOpenStoryboard = Resources["AlbumBoxOpenStoryboard"] as Storyboard
            ?? throw new ArgumentNullException("未找到 AlbumBoxOpenStoryboard 资源");

        _audioManager = App.GetRequiredService<IAudioManager>();

        _audioManager.AudioPlaying += OnPlaying;
        _audioManager.AudioPaused += OnPaused;
        _audioManager.AudioStopped += OnStopped;
    }

    public Storyboard AlbumOpenStoryboard;
    public Storyboard AlbumRotateStoryboard;
    public Storyboard AlbumBoxOpenStoryboard;

    public ImmersiveVinylPresenterViewModel ViewModel { get; set; }

    private void OnPlaying(object? sender, AudioPlayingArgs e)
    {
        if (e.PlaybackState is PlaybackState.Paused)
        {
            AlbumRotateStoryboard.Resume();
        }
        else
        {
            AlbumRotateStoryboard.Begin();
        }

        if (e.PlaybackState is PlaybackState.Stopped)
        {
            AlbumBoxOpenStoryboard.Begin();
        }

        var keyFramesAnimation = (DoubleAnimationUsingKeyFrames)AlbumOpenStoryboard.Children[0];

        keyFramesAnimation.KeyFrames[0].Value = 0;
        keyFramesAnimation.KeyFrames[1].Value = _transformFactor;

        AlbumOpenStoryboard.Begin(Album, false);
    }

    private void OnPaused(object? sender, EventArgs e)
    {
        AlbumRotateStoryboard.Pause();

        var keyFramesAnimation = (DoubleAnimationUsingKeyFrames)AlbumOpenStoryboard.Children[0];

        keyFramesAnimation.KeyFrames[0].Value = _transformFactor;
        keyFramesAnimation.KeyFrames[1].Value = 0;

        AlbumOpenStoryboard.Begin(Album, false);
    }

    private void OnStopped(object? sender, EventArgs e)
    {
        AlbumRotateStoryboard.Stop();

        var keyFramesAnimation = (DoubleAnimationUsingKeyFrames)AlbumOpenStoryboard.Children[0];

        keyFramesAnimation.KeyFrames[0].Value = _transformFactor;
        keyFramesAnimation.KeyFrames[1].Value = 0;

        AlbumOpenStoryboard.Begin(Album, false);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged += OnSizeChanged;

        var width = Math.Max(300, AppCore.MainWindow.ActualWidth / 4);

        var lastCoverWidth = ViewModel.CurrentCoverWidth;

        ViewModel.CurrentVinyWidth = width * 280 / 300;
        ViewModel.CurrentCoverWidth = width;
        ViewModel.CurrentRotateCoverWidth = width * 0.6;
        ViewModel.CurrentRotateCoverCanvasTop = (width - ViewModel.CurrentVinyWidth) / 2;
        MagneticCircle.Width = width / 3;
        MagneticCircle.Height = MagneticCircle.Width;
        MagneticMiddleCircle.Width = width / 10;
        MagneticMiddleCircle.Height = MagneticMiddleCircle.Width;

        var currentWindowHeight = AppCore.MainWindow.ActualHeight;
        var currentTopMargin = Math.Max(0, ((currentWindowHeight - width) / 5) - 100);

        ViewModel.CurrentPanelMargin = new Thickness(0, currentTopMargin, 0, 0);

        _transformFactor = 160 + ((lastCoverWidth - 300) * 160 / 300);

        var animation = new DoubleAnimation()
        {
            From = AlbumPop.X,
            To = _transformFactor,
            Duration = TimeSpan.FromMicroseconds(100)
        };

        AlbumPop.BeginAnimation(TranslateTransform.XProperty, animation);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        AppCore.MainWindow.SizeChanged -= OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var width = Math.Max(300, e.NewSize.Width / 4);

        var lastCoverWidth = ViewModel.CurrentCoverWidth;

        ViewModel.CurrentVinyWidth = width * 280 / 300;
        ViewModel.CurrentCoverWidth = width;
        ViewModel.CurrentRotateCoverWidth = width * 0.6;
        ViewModel.CurrentRotateCoverCanvasTop = (width - ViewModel.CurrentVinyWidth) / 2;
        MagneticCircle.Width = width / 3;
        MagneticCircle.Height = MagneticCircle.Width;
        MagneticMiddleCircle.Width = width / 10;
        MagneticMiddleCircle.Height = MagneticMiddleCircle.Width;

        var currentWindowHeight = AppCore.MainWindow.ActualHeight;
        var currentTopMargin = Math.Max(0, ((currentWindowHeight - width) / 5) - 100);

        ViewModel.CurrentPanelMargin = new Thickness(0, currentTopMargin, 0, 0);

        _transformFactor = 160 + ((lastCoverWidth - 300) * 160 / 300);

        var animation = new DoubleAnimation()
        {
            From = AlbumPop.X,
            To = _transformFactor,
            Duration = TimeSpan.FromMicroseconds(100)
        };

        AlbumPop.BeginAnimation(TranslateTransform.XProperty, animation);
    }
}