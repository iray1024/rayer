using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace Rayer.Core.Controls;

public partial class MaskContentDialog : ContentDialog
{
    public MaskContentDialog(ContentPresenter? contentPresenter)
        : base(contentPresenter)
    {
        InitializeComponent();

        TitleTemplate = (DataTemplate)Resources["TitleTemplate"];
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.RegisterAttached(
            "Description",
            typeof(string),
            typeof(MaskContentDialog),
            new PropertyMetadata(null));

    public static string GetDescription(DependencyObject obj)
    {
        return (string)obj.GetValue(DescriptionProperty);
    }

    public static void SetDescription(DependencyObject obj, string value)
    {
        obj.SetValue(DescriptionProperty, value);
    }

    private void OnPreviewCloseMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        e.Handled = true;

        VisualStateManager.GoToState(this, "Close", true);

        Hide();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "Open", true);

        var mask = (Rectangle)((Control)sender).Template.FindName("Mask", (Control)sender);

        mask.MouseLeftButtonUp += OnMaskMouseLeftButtonUp;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        var mask = (Rectangle)((Control)sender).Template.FindName("Mask", (Control)sender);

        mask.MouseLeftButtonUp -= OnMaskMouseLeftButtonUp;
    }

    private void OnMaskMouseLeftButtonUp(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        VisualStateManager.GoToState(this, "Close", true);

        Hide();
    }
}