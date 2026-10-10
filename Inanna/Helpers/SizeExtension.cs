using Avalonia;
using Gaia.Models;

namespace Inanna.Helpers;

public static class SizeExtension
{
    public static GSize ToGSize(this Size size)
    {
        return new GSize(UnitLength.FromPixel(size.Width), UnitLength.FromPixel(size.Height));
    }
}
