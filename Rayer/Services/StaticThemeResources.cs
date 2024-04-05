using System.Windows.Media.Imaging;

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
    }

    internal static readonly BitmapImage AlbumFallback = new(new Uri("pack://application:,,,/assets/fallback.png"));
}