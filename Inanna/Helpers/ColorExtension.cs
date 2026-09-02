using Avalonia.Media;

namespace Inanna.Helpers;

public static class ColorExtension
{
    public static SolidColorBrush ToSolidColorBrush(this Color color)
    {
        return new(color);
    }
}
