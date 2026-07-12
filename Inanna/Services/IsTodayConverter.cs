using System.Globalization;
using Avalonia.Data.Converters;
using Gaia.Helpers;

namespace Inanna.Services;

public sealed class IsTodayConverter : IValueConverter
{
    public static readonly IsTodayConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateOnly date)
        {
            return date == DateTime.Now.ToDateOnly();
        }

        if (value is DateTime dateTime)
        {
            return dateTime.ToDateOnly() == DateTime.Now.ToDateOnly();
        }

        return value;
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
