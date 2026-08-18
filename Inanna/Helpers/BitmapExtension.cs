using System.IO.Compression;
using Avalonia;
using Avalonia.Media.Imaging;

namespace Inanna.Helpers;

public static class BitmapExtension
{
    public static Stream ToStream(this Bitmap bitmap)
    {
        var stream = new MemoryStream();

        bitmap.Save(
            stream,
            new PngBitmapEncoderOptions { CompressionLevel = CompressionLevel.SmallestSize }
        );

        stream.Position = 0;

        return stream;
    }

    public static Bitmap DecodeToWidth(this Bitmap bitmap, int width)
    {
        using var stream = bitmap.ToStream();

        return Bitmap.DecodeToWidth(stream, width);
    }

    public static Bitmap Rotate(this Bitmap source, int angleDegrees)
    {
        // For 90 or 270 degree flips, swap width and height
        var is90or270 = Math.Abs(angleDegrees % 180) == 90;
        var targetWidth = is90or270 ? (int)source.Size.Height : (int)source.Size.Width;
        var targetHeight = is90or270 ? (int)source.Size.Width : (int)source.Size.Height;

        var targetSize = new PixelSize(targetWidth, targetHeight);

        var renderTarget = new RenderTargetBitmap(targetSize);
        using var context = renderTarget.CreateDrawingContext();
        // Move pivot to origin -> rotate -> move back to fit boundaries
        var moveCentreToOrigin = Matrix.CreateTranslation(
            -source.Size.Width / 2.0,
            -source.Size.Height / 2.0
        );
        var rotate = Matrix.CreateRotation(angleDegrees * Math.PI / 180.0);
        var moveToRotatedCentre = Matrix.CreateTranslation(targetWidth / 2.0, targetHeight / 2.0);

        // Combine matrices and push the transform
        var transformation = moveCentreToOrigin * rotate * moveToRotatedCentre;

        using (context.PushTransform(transformation))
        {
            // Draw the original bitmap into the context
            context.DrawImage(
                source,
                new Rect(0, 0, source.Size.Width, source.Size.Height),
                new Rect(0, 0, source.Size.Width, source.Size.Height)
            );
        }

        // Return the newly drawn, rotated bitmap
        return renderTarget;
    }
}
