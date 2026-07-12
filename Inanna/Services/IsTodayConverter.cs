using System.Globalization;
using Avalonia.Data.Converters;
using Gaia.Helpers;

namespace Inanna.Services;

public sealed class IsTodayWithoutYearConverter : IValueConverter
{
    public static readonly IsTodayWithoutYearConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var now = DateTime.Now.ToDateOnly();

        if (value is DateOnly date)
        {
            var day = Math.Min(DateTime.DaysInMonth(now.Year, date.Month), date.Day);

            return new DateOnly(now.Year, date.Month, day) == now;
        }

        if (value is DateTime dateTime)
        {
            var day = Math.Min(DateTime.DaysInMonth(now.Year, dateTime.Month), dateTime.Day);

            return new DateOnly(now.Year, dateTime.Month, day) == now;
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
