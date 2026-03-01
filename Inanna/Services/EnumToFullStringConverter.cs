using System.Globalization;
using Avalonia.Data.Converters;

namespace Inanna.Services;

public sealed class EnumToFullStringConverter : IValueConverter
{
    public static readonly EnumToFullStringConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Enum e)
        {
            return value;
        }

        return $"{System.Convert.ToInt32(e)} {EnumLocalizationConverter.Instance.Convert(value, targetType, parameter, culture)}";
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not string str)
        {
            return value;
        }

        var val = str.Split(' ')[0];

        return Enum.Parse(targetType, val);
    }
}
