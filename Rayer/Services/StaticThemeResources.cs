using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wpf.Ui.Appearance;

namespace Rayer.Services;

internal static class StaticThemeResources
{
    internal static class Light
    {
        internal static readonly BitmapImage Play = new(new Uri("pack://application:,,,/assets/light/play.png"));
        internal static readonly BitmapImage Pause = new(new Uri("pack://application:,,,/assets/light/pause.png"));
        internal static readonly BitmapImage Previous = new(new Uri("pack://application:,,,/assets/light/previous.png"));
        internal static readonly BitmapImage Next = new(new Uri("pack://application:,,,/assets/light/next.png"));
        internal static readonly BitmapImage Repeat = new(new Uri("pack://application:,,,/assets/light/repeat.png"));
        internal static readonly BitmapImage Shuffle = new(new Uri("pack://application:,,,/assets/light/shuffle.png"));
        internal static readonly BitmapImage Single = new(new Uri("pack://application:,,,/assets/light/single.png"));
        internal static readonly BitmapImage VolumeFull = new(new Uri("pack://application:,,,/assets/light/volume-full.png"));
        internal static readonly BitmapImage VolumeHalf = new(new Uri("pack://application:,,,/assets/light/volume-half.png"));
        internal static readonly BitmapImage Mute = new(new Uri("pack://application:,,,/assets/light/mute.png"));
        internal static readonly BitmapImage Pitch = new(new Uri("pack://application:,,,/assets/light/pitch.png"));
        internal static readonly BitmapImage PlayQueue = new(new Uri("pack://application:,,,/assets/light/play-queue.png"));
        internal static readonly BitmapImage Equalizer = new(new Uri("pack://application:,,,/assets/light/equalizer.png"));
        internal static readonly BitmapImage Speed = new(new Uri("pack://application:,,,/assets/light/speed.png"));

        internal static readonly SolidColorBrush TextFillColorPrimaryBrush = new(Color.FromArgb(0xe4, 0, 0, 0));
        internal static readonly SolidColorBrush SliderTrackFill = new(Color.FromArgb(0x72, 0, 0, 0));
    }

    internal static class Dark
    {
        internal static readonly BitmapImage Play = new(new Uri("pack://application:,,,/assets/dark/play.png"));
        internal static readonly BitmapImage Pause = new(new Uri("pack://application:,,,/assets/dark/pause.png"));
        internal static readonly BitmapImage Previous = new(new Uri("pack://application:,,,/assets/dark/previous.png"));
        internal static readonly BitmapImage Next = new(new Uri("pack://application:,,,/assets/dark/next.png"));
        internal static readonly BitmapImage Repeat = new(new Uri("pack://application:,,,/assets/dark/repeat.png"));
        internal static readonly BitmapImage Shuffle = new(new Uri("pack://application:,,,/assets/dark/shuffle.png"));
        internal static readonly BitmapImage Single = new(new Uri("pack://application:,,,/assets/dark/single.png"));
        internal static readonly BitmapImage VolumeFull = new(new Uri("pack://application:,,,/assets/dark/volume-full.png"));
        internal static readonly BitmapImage VolumeHalf = new(new Uri("pack://application:,,,/assets/dark/volume-half.png"));
        internal static readonly BitmapImage Mute = new(new Uri("pack://application:,,,/assets/dark/mute.png"));
        internal static readonly BitmapImage Pitch = new(new Uri("pack://application:,,,/assets/dark/pitch.png"));
        internal static readonly BitmapImage PlayQueue = new(new Uri("pack://application:,,,/assets/dark/play-queue.png"));
        internal static readonly BitmapImage Equalizer = new(new Uri("pack://application:,,,/assets/dark/equalizer.png"));
        internal static readonly BitmapImage Speed = new(new Uri("pack://application:,,,/assets/dark/speed.png"));

        internal static readonly SolidColorBrush TextFillColorPrimaryBrush = new(Color.FromRgb(255, 255, 255));
        internal static readonly SolidColorBrush SliderTrackFill = new(Color.FromArgb(0x8b, 255, 255, 255));
    }

    internal static readonly BitmapImage AlbumFallback = new(new Uri("pack://application:,,,/assets/fallback.png"));

    internal static object GetDynamicResource(string resourceKey)
    {
        return Application.Current.Resources[resourceKey];
    }

    internal static object? GetResource(string resourceKey, ApplicationTheme applicationTheme = ApplicationTheme.Light)
    {
        var app = Application.Current;

        var resources = GetResourceDictionary(applicationTheme) ?? app.Resources;

        return resources.Contains(resourceKey)
            ? resources[resourceKey]
            : null;
    }

    private static ResourceDictionary? GetResourceDictionary(ApplicationTheme applicationTheme)
    {
        var resources = Application.Current.Resources;

        foreach (var dictionary in resources.MergedDictionaries)
        {
            if (dictionary.Source is not null && dictionary.Source.ToString().Contains(applicationTheme.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return dictionary;
            }
        }

        return null;
    }
}