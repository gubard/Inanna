using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Inanna.Converters;

public class DoubleToPercentsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double number)
        {
            return value;
        }

        if (!TryGetPercentage(parameter, out var percentage))
        {
            return value;
        }

        return number * percentage;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double number)
        {
            return value;
        }

        if (!TryGetPercentage(parameter, out var percentage))
        {
            return value;
        }

        return number / percentage;
    }

    private static bool TryGetPercentage([NotNullWhen(true)] object? parameter, out double percentage)
    {
        if (parameter is double value)
        {
            percentage = value;

            return true;
        }

        return double.TryParse(parameter?.ToString(), CultureInfo.InvariantCulture.NumberFormat, out percentage);
    }
}