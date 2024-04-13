using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.ViewModels.Explore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace Rayer.SearchEngine.Views.Pages.Explore;

[Inject]
public partial class ExploreHomePage : INavigableView<ExploreHomeViewModel>
{
    public ExploreHomePage()
    {
        var vm = AppCore.GetRequiredService<ExploreHomeViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();
    }

    public ExploreHomeViewModel ViewModel { get; set; }
}