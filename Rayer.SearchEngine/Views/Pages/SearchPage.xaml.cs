using Rayer.Core;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Models.Response.Search;
using Rayer.SearchEngine.ViewModels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Wpf.Ui.Controls;

namespace Rayer.SearchEngine.Views.Pages;

[Inject(ServiceLifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped)]
public partial class SearchPage : INavigableView<SearchViewModel>
{
    private readonly Storyboard _titlebarControlStoryboard = new();

    public SearchPage()
    {
        var vm = AppCore.GetRequiredService<SearchViewModel>();

        ViewModel = vm;

        InitializeComponent();
    }

    public SearchViewModel ViewModel { get; set; }

    private async void OnTitleBarControlMouseEnter(object sender, MouseEventArgs e)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _titlebarControlStoryboard.Stop();
            _titlebarControlStoryboard.Children.Clear();

            var colorAnimation = new ColorAnimation()
            {
                From = Color.FromArgb(0, 0, 0, 0),
                To = (Color)Application.Current.Resources["ControlStrokeColorDefault"],
                Duration = TimeSpan.FromMilliseconds(200)
            };

            Storyboard.SetTarget(colorAnimation, sender as DependencyObject);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Border.Background).(SolidColorBrush.Color)"));

            _titlebarControlStoryboard.Children.Add(colorAnimation);

            Timeline.SetDesiredFrameRate(_titlebarControlStoryboard, 60);

            _titlebarControlStoryboard.Begin();
        });
    }

    private async void OnTitleBarControlMouseLeave(object sender, MouseEventArgs e)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _titlebarControlStoryboard.Stop();
            _titlebarControlStoryboard.Children.Clear();

            var colorAnimation = new ColorAnimation()
            {
                From = (Color)Application.Current.Resources["ControlStrokeColorDefault"],
                To = Color.FromArgb(0, 0, 0, 0),
                Duration = TimeSpan.FromMilliseconds(200)
            };

            Storyboard.SetTarget(colorAnimation, sender as DependencyObject);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Border.Background).(SolidColorBrush.Color)"));

            _titlebarControlStoryboard.Children.Add(colorAnimation);

            Timeline.SetDesiredFrameRate(_titlebarControlStoryboard, 60);

            _titlebarControlStoryboard.Begin();
        });
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is SearchAggregationModel model)
        {
            ViewModel.Model = model;

            DataContext = this;
        }
    }

    private void OnCheckedChanged(object sender, RoutedEventArgs e)
    {
        
    }
}