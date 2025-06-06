﻿using Rayer.SearchEngine.Core.Domain.Artist;
using System.Globalization;
using System.Windows.Data;

namespace Rayer.SearchEngine.Converters;

internal sealed class WebArtistsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is ArtistDefinition[] details
            ? string.Join(" / ", details.Select(x => x.Name))
            : "Unknown";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}