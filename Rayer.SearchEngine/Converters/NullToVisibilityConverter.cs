﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rayer.SearchEngine.Converters;

public class NullToVisibilityConverter : IValueConverter
{
    private static readonly object _nullVisibility = Visibility.Collapsed;
    private static readonly object _notNullVisibility = Visibility.Visible;


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is null ? _nullVisibility : _notNullVisibility;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}