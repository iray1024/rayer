using System.Globalization;
using System.Windows.Data;

namespace Rayer.SearchEngine.Converters;

internal class PlaylistDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string description)
        {
            var slices = description.Split('\n');

            if (slices.Length >= 3)
            {
                return parameter?.Equals("0") == true
                    ? slices[0]
                    : parameter?.Equals("1") == true
                        ? slices[1]
                        : slices[2];
            }
            else if (slices.Length == 2)
            {
                return parameter?.Equals("0") == true
                    ? slices[0]
                    : parameter?.Equals("1") == true
                        ? slices[1]
                        : string.Empty;
            }
            else if (slices.Length == 1)
            {
                return parameter?.Equals("0") == true
                    ? slices[0]
                    : string.Empty;
            }
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}