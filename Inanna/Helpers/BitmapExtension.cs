using System.IO.Compression;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Inanna.Helpers;

public static class BitmapExtension
{
    public static WriteableBitmap ToGrayscale(this Bitmap source)
    {
        var pixelSize = source.PixelSize;
        var dpi = source.Dpi;

        var writable = new WriteableBitmap(
            pixelSize,
            dpi,
            PixelFormat.Bgra8888,
            AlphaFormat.Premul
        );

        var width = pixelSize.Width;
        var height = pixelSize.Height;
        var stride = width * 4;
        var pixelData = new byte[height * stride];

        unsafe
        {
            fixed (byte* pPixels = pixelData)
            {
                source.CopyPixels(
                    new PixelRect(0, 0, width, height),
                    (nint)pPixels,
                    pixelData.Length,
                    stride
                );
            }

            for (var i = 0; i < pixelData.Length; i += 4)
            {
                var b = pixelData[i];
                var g = pixelData[i + 1];
                var r = pixelData[i + 2];

                var gray = (byte)(0.299 * r + 0.587 * g + 0.114 * b);

                pixelData[i] = gray;
                pixelData[i + 1] = gray;
                pixelData[i + 2] = gray;
            }

            using var fb = writable.Lock();
            var dstPtr = (byte*)fb.Address;
            var rowBytes = fb.RowBytes;

            fixed (byte* srcPtr = pixelData)
            {
                for (var y = 0; y < height; y++)
                {
                    Buffer.MemoryCopy(
                        srcPtr + (y * stride),
                        dstPtr + (y * rowBytes),
                        rowBytes,
                        stride
                    );
                }
            }
        }

        return writable;
    }

    public static Stream ToStreamNoCompression(this Bitmap bitmap)
    {
        var stream = new MemoryStream();

        bitmap.Save(
            stream,
            new PngBitmapEncoderOptions { CompressionLevel = CompressionLevel.NoCompression }
        );

        stream.Position = 0;

        return stream;
    }

    public static Bitmap DecodeToWidthNoCompression(this Bitmap bitmap, int width)
    {
        using var stream = bitmap.ToStreamNoCompression();

        return Bitmap.DecodeToWidth(stream, width);
    }

    public static Stream ToStreamSmallestSize(this Bitmap bitmap)
    {
        var stream = new MemoryStream();

        bitmap.Save(
            stream,
            new PngBitmapEncoderOptions { CompressionLevel = CompressionLevel.SmallestSize }
        );

        stream.Position = 0;

        return stream;
    }

    public static Bitmap DecodeToWidthSmallestSize(this Bitmap bitmap, int width)
    {
        using var stream = bitmap.ToStreamSmallestSize();

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
