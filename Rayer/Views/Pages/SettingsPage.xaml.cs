using Rayer.ViewModels;
using Wpf.Ui.Controls;

namespace Rayer.Views.Pages;

public partial class SettingsPage : INavigableView<SettingsViewModel>
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    public SettingsViewModel ViewModel { get; }
}