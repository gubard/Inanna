using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Inanna.Services;

public class Int32MoreThenConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int x)
        {
            return value;
        }

        if (!TryGetY(parameter, out var y))
        {
            return value;
        }

        return x > y;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static bool TryGetY([NotNullWhen(true)] object? parameter, out int y)
    {
        if (parameter is not int value)
        {
            return int.TryParse(parameter?.ToString(), CultureInfo.InvariantCulture.NumberFormat, out y);
        }

        y = value;

        return true;
    }
}