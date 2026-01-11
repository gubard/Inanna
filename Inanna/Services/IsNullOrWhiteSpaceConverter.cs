using System.Globalization;
using Avalonia.Data.Converters;
using Gaia.Helpers;

namespace Inanna.Services;

public sealed class IsNullOrWhiteSpaceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str)
        {
            return value;
        }

        return str.IsNullOrWhiteSpace();
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException();
    }
}