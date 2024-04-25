using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Rayer.SearchEngine.Controls;

public partial class SampleContentDialog : ContentDialog
{
    public SampleContentDialog(ContentPresenter? contentPresenter)
        : base(contentPresenter)
    {
        InitializeComponent();

        TitleTemplate = (DataTemplate)Resources["TitleTemplate"];
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.RegisterAttached(
            "Description",
            typeof(string),
            typeof(SampleContentDialog),
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

        Hide();
    }
}