using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Gaia.Helpers;
using Inanna.Helpers;

namespace Inanna.Services;

public sealed class StringLocalizationValueConverter : IValueConverter
{
    private readonly Application _app = Application.Current.ThrowIfNull();

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
}
