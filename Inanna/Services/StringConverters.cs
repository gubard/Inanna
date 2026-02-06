using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Gaia.Helpers;
using Inanna.Helpers;

namespace Inanna.Services;

public sealed class StringLocalizationConverter : IValueConverter
{
    public static readonly StringLocalizationConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        if (value is not string str)
        {
            return value;
        }

        var resource = _app.GetResourceOrNull($"Lang.{str}");

        return resource ?? value;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException();
    }

    private readonly Application _app = Application.Current.ThrowIfNull();
}

public sealed class IsNullOrWhiteSpaceConverter : IValueConverter
{
    public static readonly IsNullOrWhiteSpaceConverter Instance = new();

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

public sealed class IsNotNullOrWhiteSpaceConverter : IValueConverter
{
    public static readonly IsNotNullOrWhiteSpaceConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str)
        {
            return value;
        }

        return !str.IsNullOrWhiteSpace();
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
