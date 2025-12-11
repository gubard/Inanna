using System.Globalization;
using Avalonia.Data.Converters;
using Gaia.Helpers;

namespace Inanna.Services;

public sealed class DateOnlyToDateTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateOnly date)
        {
            return value;
        }

        return date.ToDateTime(TimeOnly.MinValue);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTime date)
        {
            return value;
        }

        return date.ToDateOnly();
    }
}