using System.Collections;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Inanna.Services;

public sealed class AnyOfConverter : IValueConverter
{
    public static readonly AnyOfConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not IEnumerable enumerable)
        {
            return value;
        }

        return enumerable.OfType<object>().Any(x => x.Equals(value));
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

public sealed class IsValueConverter : IValueConverter
{
    public static readonly IsValueConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return parameter is null;
        }

        return value.Equals(parameter);
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

public sealed class IsNotValueConverter : IValueConverter
{
    public static readonly IsNotValueConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return parameter is not null;
        }

        return !value.Equals(parameter);
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
