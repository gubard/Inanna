using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Inanna.Services;

public sealed class Int32MoreThenConverter : IValueConverter
{
    public static readonly Int32MoreThenConverter Instance = new();

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

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException();
    }

    private static bool TryGetY([NotNullWhen(true)] object? parameter, out int y)
    {
        if (parameter is not int value)
        {
            return int.TryParse(
                parameter?.ToString(),
                CultureInfo.InvariantCulture.NumberFormat,
                out y
            );
        }

        y = value;

        return true;
    }
}

public sealed class Int32MoreThanConverter : IValueConverter
{
    public static readonly Int32MoreThanConverter Instance = new();

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
