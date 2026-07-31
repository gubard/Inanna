using System.Globalization;
using Avalonia.Data.Converters;

namespace Inanna.Services;

public sealed class HasFlagConverter : IValueConverter
{
    public static readonly HasFlagConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var flags = System.Convert.ToUInt64(value);
        var flag = System.Convert.ToUInt64(parameter);

        return (flags & flag) == flag;
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
