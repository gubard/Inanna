using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Gaia.Helpers;
using Inanna.Helpers;

namespace Inanna.Converters;

public sealed class EnumTypeToValuesConverter : IValueConverter
{
    public static readonly EnumTypeToValuesConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Type type)
        {
            return value;
        }

        return EnumHelper.GetValues(type);
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not Enum[] enums)
        {
            return value;
        }

        return enums[0].GetType();
    }
}

public sealed class EnumValueToValuesConverter : IValueConverter
{
    public static readonly EnumValueToValuesConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Enum e)
        {
            return value;
        }

        return EnumHelper.GetValues(e.GetType());
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

public sealed class EnumLocalizationConverter : IValueConverter
{
    public static readonly EnumLocalizationConverter Instance = new();

    private readonly Application _app = Application.Current.ThrowIfNull();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        if (value is not Enum e)
        {
            return value;
        }

        var resource = _app.GetResourceOrNull($"Lang.{e}");

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
}
