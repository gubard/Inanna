using System.Collections;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Inanna.Services;

public class InsertFirstElementConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        if (value is not IEnumerable enumerable)
        {
            return null;
        }

        return CreateNewEnumerable(enumerable, parameter);
    }

    public object? ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        if (value is not IEnumerable enumerable)
        {
            return null;
        }

        return BackEnumerable(enumerable);
    }

    private IEnumerable CreateNewEnumerable(IEnumerable enumerable,
        object? parameter)
    {
        yield return parameter;

        foreach (var item in enumerable)
        {
            yield return item;
        }
    }

    private IEnumerable BackEnumerable(IEnumerable enumerable)
    {
        var enumerator = enumerable.GetEnumerator();
        enumerator.MoveNext();

        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }

        enumerator.Reset();
    }
}