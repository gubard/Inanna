using Avalonia;
using Avalonia.Controls;

namespace Inanna.Helpers;

public static class ScrollViewerExtension
{
    public static void FixScrollViewerInitOffset(this ScrollViewer scrollViewer)
    {
        var count = 0;
        IDisposable? scrollViewerOffsetChangedHandler = null;

        scrollViewerOffsetChangedHandler =
            ScrollViewer.OffsetProperty.Changed.AddClassHandler<ScrollViewer>(
                (c, _) =>
                {
                    if (c != scrollViewer)
                    {
                        return;
                    }

                    count++;

                    if (count != 2)
                    {
                        return;
                    }

                    scrollViewer.Offset = Vector.Zero;
                    scrollViewerOffsetChangedHandler?.Dispose();
                }
            );
    }
}
