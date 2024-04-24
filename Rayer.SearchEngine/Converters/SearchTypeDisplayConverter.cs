using Rayer.SearchEngine.Core.Enums;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.SearchEngine.Converters;

internal class SearchTypeDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is SearchType searchType
            ? searchType switch
            {
                SearchType.Audio => "Track",
                SearchType.Artist => "Artist",
                SearchType.Album => "Album",
                SearchType.Video => "Video",
                SearchType.Playlist => "Playlist",
                _ => "Playlist",
            }
            : value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}