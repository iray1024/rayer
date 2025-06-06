using Rayer.FrameworkCore;
using Rayer.FrameworkCore.Injection;
using Rayer.Updater.ViewModels;
using System.Windows;
using Wpf.Ui;

namespace Rayer.Updater;

[Inject<IWindow>(ResolveServiceType = true)]
public partial class MainWindow : IWindow
{
    public MainWindow(MainWindowViewModel viewModel, IContentDialogService contentDialogService)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        contentDialogService.SetDialogHost(RootContentDialog);

        Loaded += OnLoaded;
    }

    public MainWindowViewModel ViewModel { get; set; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Task.Run(() => ViewModel.CheckUpdateAsync(AppCore.StoppingToken), AppCore.StoppingToken);
    }
}