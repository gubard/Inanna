using System.Collections;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Inanna.Services;

public class AnyOfConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not IEnumerable enumerable)
        {
            return value;
        }

        return enumerable.OfType<object>().Any(x => x.Equals(value));
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
