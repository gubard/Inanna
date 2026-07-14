using System.Globalization;
using Avalonia.Data.Converters;
using Gaia.Helpers;

namespace Inanna.Services;

public record struct DateOptions
{
    public required byte MinDays { get; init; }
    public required byte MaxDays { get; init; }
}

public sealed class SometimeAroundNowWithoutYearConverter : IValueConverter
{
    public static readonly SometimeAroundNowWithoutYearConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var now = DateTime.Now.ToDateOnly();
        var date = GetDate(value, now.Year);

        if (!date.HasValue)
        {
            return value;
        }

        if (parameter is not DateOptions options)
        {
            return value;
        }

        var min = date.Value.AddDays(-options.MinDays);
        var max = date.Value.AddDays(options.MaxDays);

        return min <= now && max >= now;
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

    private DateOnly? GetDate(object? value, int year)
    {
        if (value is DateOnly date)
        {
            var day = Math.Min(DateTime.DaysInMonth(year, date.Month), date.Day);

            return new DateOnly(year, date.Month, day);
        }

        if (value is DateTime dateTime)
        {
            var day = Math.Min(DateTime.DaysInMonth(year, dateTime.Month), dateTime.Day);

            return new DateOnly(year, dateTime.Month, day);
        }

        return null;
    }
}

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
