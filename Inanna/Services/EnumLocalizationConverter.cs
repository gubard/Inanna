using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Gaia.Helpers;
using Inanna.Helpers;

namespace Inanna.Services;

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
