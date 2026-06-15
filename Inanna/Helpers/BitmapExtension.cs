using Avalonia.Media.Imaging;

namespace Inanna.Helpers;

public static class BitmapExtension
{
    public static Stream ToStream(this Bitmap bitmap)
    {
        var stream = new MemoryStream();
        bitmap.Save(stream);
        stream.Position = 0;

        return stream;
    }
}
