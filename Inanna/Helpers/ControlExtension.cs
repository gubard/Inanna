using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace Inanna.Helpers;

public static class ControlExtension
{
    public static Stream ToPngStream(this Control control)
    {
        var output = new MemoryStream();
        var oldSize = Dispatcher.UIThread.Invoke(() => new Size(control.Width, control.Height));

        Dispatcher.UIThread.Invoke(() =>
        {
            var size = control.Bounds.Size;
            control.Width = size.Width;
            control.Height = size.Height;
        });

        Dispatcher.UIThread.Invoke(() =>
        {
            var size = control.Bounds.Size;

            if (size.Width <= 0 || size.Height <= 0)
                throw new InvalidOperationException(
                    "Control has no valid size. Make sure it is loaded and has non-zero Bounds, "
                        + "or set Width/Height and call Measure/Arrange."
                );

            var pixelSize = new PixelSize(
                (int)Math.Ceiling(size.Width),
                (int)Math.Ceiling(size.Height)
            );

            var dpi = new Vector(96, 96);
            using var rtb = new RenderTargetBitmap(pixelSize, dpi);
            rtb.Render(control);
            rtb.Save(output);
            control.Width = oldSize.Width;
            control.Height = oldSize.Height;
        });

        output.Position = 0;

        return output;
    }
}
