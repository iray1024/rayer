using Rayer.Core.Abstractions;
using Rayer.Core.Models;
using Rayer.SearchEngine.ViewModels;
using System.Windows;
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
            Title = "一笑江湖",
            Artists = ["姜姜"],
            Path = "http://m8.music.126.net/20240413040827/88de232d9d81c0e89f20107afda06b70/ymusic/obj/w5zDlMODwrDDiGjCn8Ky/2696902133/cb84/fa1d/89d4/4a19746870f59373124d76576b7d2d90.mp3"
        };

        await _audioManager.Playback.Play(audio);
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        //await ViewModel.LoadAsAnonymousAsync();

        //var loginWindow = AppCore.GetRequiredService<LoginWindow>();
        //loginWindow.Show();
    }
}