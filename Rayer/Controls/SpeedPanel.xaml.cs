using Rayer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Rayer.Controls;

public partial class SpeedPanel : UserControl
{
    public SpeedPanel()
    {
        var vm = App.GetRequiredService<SpeedPanelViewModel>();

        ViewModel = vm;
        DataContext = this;

        InitializeComponent();

        ViewModel.SetDependency(Speed);
    }

    public SpeedPanelViewModel ViewModel { get; set; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var popup = (Popup)Flyout.Template.FindName("PART_Popup", Flyout);

        if (popup is not null)
        {
            popup.CustomPopupPlacementCallback = new CustomPopupPlacementCallback((popupSize, targetSize, offset) =>
            {
                return [new(
                        new Point(
                            -targetSize.Width + 8,
                            (popupSize.Height * -1) + 6),
                        PopupPrimaryAxis.Vertical)];

            });
        }
        else
        {
            Flyout.Placement = PlacementMode.Top;
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        ViewModel.OnButtonClick();
    }

    private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        ViewModel.OnButtonRightClick();
    }

    #region Slider Events
    private void Slider_DragDelta(object sender, DragDeltaEventArgs e)
    {
        ViewModel.SetSpeed();
    }

    private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        ViewModel.SetSpeed();
        ViewModel.Save();
    }

    private void Slider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        var value = 100 - (e.GetPosition(SpeedSlider).Y / SpeedSlider.ActualHeight * (SpeedSlider.Maximum - SpeedSlider.Minimum));

        ViewModel.SetSpeed((float)value);
        ViewModel.Save();
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var value = ViewModel.Speed + (e.Delta / 20.0f);

        value = Math.Min(Math.Max(value, 0f), 100);

        ViewModel.SetSpeed(value);
        ViewModel.Save();
    }
    #endregion
}