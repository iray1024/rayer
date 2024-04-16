using Microsoft.Extensions.DependencyInjection;
using Rayer.Installer.ViewModels;

namespace Rayer.Installer.Transitions;

public partial class Slide1 : System.Windows.Controls.UserControl
{
    public Slide1()
    {
        InitializeComponent();

        DataContext = App.Services.GetRequiredService<MainViewModel>();
    }
}