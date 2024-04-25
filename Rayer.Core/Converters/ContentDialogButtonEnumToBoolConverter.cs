using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Controls;

namespace Rayer.Core.Converters;

internal class ContentDialogButtonEnumToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not ContentDialogButton valueEnum
            ? throw new ArgumentException($"{nameof(value)} 不是类型: {typeof(ContentDialogButton)}")
            : parameter is not ContentDialogButton parameterEnum
            ? throw new ArgumentException($"{nameof(parameter)} 不是类型: {typeof(ContentDialogButton)}")
            : (object)EqualityComparer<ContentDialogButton>.Default.Equals(valueEnum, parameterEnum);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}