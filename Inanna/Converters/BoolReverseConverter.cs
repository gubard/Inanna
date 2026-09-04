using System.Globalization;
using Avalonia.Data.Converters;

namespace Inanna.Converters;

public sealed class BoolReverseConverter : IValueConverter
{
    public static readonly BoolReverseConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool b)
        {
            return value;
        }

        return !b;
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not bool b)
        {
            return value;
        }

        return !b;
    }
}
