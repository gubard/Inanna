using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Inanna.Services;

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

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
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

    private static bool TryGetPercentage(
        [NotNullWhen(true)] object? parameter,
        out double percentage
    )
    {
        if (parameter is double value)
        {
            percentage = value;

            return true;
        }

        return double.TryParse(
            parameter?.ToString(),
            CultureInfo.InvariantCulture.NumberFormat,
            out percentage
        );
    }
}

public class Int32MoreThanConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int number)
        {
            return value;
        }

        if (!TryGet(parameter, out var value2))
        {
            return value;
        }

        return number > value2;
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

    private static bool TryGet([NotNullWhen(true)] object? parameter, out double percentage)
    {
        if (parameter is double value)
        {
            percentage = value;

            return true;
        }

        return double.TryParse(
            parameter?.ToString(),
            CultureInfo.InvariantCulture.NumberFormat,
            out percentage
        );
    }
}
