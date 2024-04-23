using Rayer.Core.Common;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.Converters;

internal class BooleanNegationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is null || !System.Convert.ToBoolean(value, CultureInfo.InvariantCulture)
            ? BooleanBoxes.TrueBox
            : BooleanBoxes.FalseBox;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}