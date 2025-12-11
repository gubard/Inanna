using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Inanna.Services;

public sealed class ColorToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Color color)
        {
            return value;
        }

        if (parameter is int a)
        {
            return new SolidColorBrush(new Color((byte)a, color.R, color.G, color.B));
        }

        return new SolidColorBrush(color);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SolidColorBrush brush)
        {
            return value;
        }

        return brush.Color;
    }
}