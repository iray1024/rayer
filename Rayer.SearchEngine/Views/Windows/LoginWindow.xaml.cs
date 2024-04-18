using Rayer.Core;
using Rayer.SearchEngine.ViewModels;
using System.Windows;

namespace Rayer.SearchEngine.Views.Windows;

public partial class LoginWindow
{
    public LoginWindow()
    {
        var vm = AppCore.GetRequiredService<LoginViewModel>();

        ViewModel = vm;
        DataContext = this;

        vm.QrCodeLoaded += OnQrCodeLoaded; ;
        vm.LoginSucceed += OnLoginSucceed;
        vm.QrCodeExpired += OnQrCodeExpired;

        InitializeComponent();
    }

    private void OnQrCodeLoaded(object? sender, EventArgs e)
    {
        Progress.Visibility = Visibility.Collapsed;
        QrCode.Visibility = Visibility.Visible;
    }

    private void OnLoginSucceed(object? sender, EventArgs e)
    {
        Close();
    }

    private async void OnQrCodeExpired(object? sender, EventArgs e)
    {
        Progress.Visibility = Visibility.Visible;
        QrCode.Visibility = Visibility.Hidden;

        await ViewModel.LoadAsync();
    }

    public LoginViewModel ViewModel { get; set; }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadAsync();
    }
}