using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace Rayer.Core.Utils;

public static class ImageIconFactory
{
    public static ImageIcon Create(string resourceName)
    {
        return Create(resourceName, new Size(20, 20), BitmapScalingMode.Fant);
    }

    public static ImageIcon Create(string resourceName, double width)
    {
        return Create(resourceName, new Size(width, width), BitmapScalingMode.Fant);
    }

    public static ImageIcon Create(string resourceName, Size size)
    {
        return Create(resourceName, size, BitmapScalingMode.Fant);
    }

    public static ImageIcon Create(string resourceName, Size size, BitmapScalingMode bitmapScalingMode)
    {
        var imageIcon = new ImageIcon()
        {
            Width = size.Width,
            Height = size.Height,
            Source = (ImageSource)Application.Current.Resources[resourceName]
        };

        RenderOptions.SetBitmapScalingMode(imageIcon, bitmapScalingMode);

        return imageIcon;
    }
}