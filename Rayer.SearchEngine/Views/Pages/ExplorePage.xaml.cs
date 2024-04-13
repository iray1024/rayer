using Rayer.Core;
using Rayer.Core.Abstractions;
using Rayer.Core.Models;
using Rayer.SearchEngine.ViewModels;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Rayer.SearchEngine.Views.Pages;

public partial class ExplorePage : INavigableView<ExploreViewModel>
{
    private readonly IAudioManager _audioManager;

    public ExplorePage(
        IAudioManager audioManager,
        ExploreViewModel viewModel)
    {
        _audioManager = audioManager;

        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public ExploreViewModel ViewModel { get; set; }

    private async void Play_Click(object sender, RoutedEventArgs e)
    {
        var audio = new Audio
        {
            Title = "旧梦一场",
            Artists = ["钟意"],
            Path = "http://m8.music.126.net/20240413094548/0eda6d451337feba7423c65edce3dccc/ymusic/obj/w5zDlMODwrDDiGjCn8Ky/14051866484/b9d6/66aa/2e32/e4059e481fb72dcead344bebdbee3f3e.mp3"
        };

        await _audioManager.Playback.Play(audio);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var navigationService = AppCore.GetRequiredService<INavigationService>();

        navigationService.SetNavigationControl(Presenter);
    }
}