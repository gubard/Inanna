using System.Globalization;
using Avalonia.Data.Converters;

namespace Inanna.Services;

public sealed class DateTimeOffsetToDateTimeConverter : IValueConverter
{
    public static readonly DateTimeOffsetToDateTimeConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTimeOffset dateTimeOffset)
        {
            return value;
        }

        return dateTimeOffset.DateTime;
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not DateTime dateTime)
        {
            return value;
        }

        return new DateTimeOffset(dateTime);
    }
}
