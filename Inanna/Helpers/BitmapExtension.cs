using System.IO.Compression;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Inanna.Helpers;

public enum GrayscaleMode
{
    Luma, // Default (0.22 R + 0.72 G + 0.06 B)
    Luminance, // Linearized sRGB weights (0.2126 R + 0.7152 G + 0.0722 B)
    Lightness, // HSL: (Max + Min) / 2
    Average, // HSI: (R + G + B) / 3
    Value, // HSV: Max(R, G, B)
}

public static class BitmapExtension
{
    public static WriteableBitmap ColorToGray(
        this Bitmap source,
        int radius = 300,
        int samples = 4,
        int iterations = 10,
        float contrast = 0,
        byte whitePoint = 200,
        byte blackPoint = 30,
        GrayscaleMode mode = GrayscaleMode.Luma
    )
    {
        var pixelSize = source.PixelSize;
        var width = pixelSize.Width;
        var height = pixelSize.Height;
        var stride = width * 4;
        var totalBytes = height * stride;

        var srcData = new byte[totalBytes];

        unsafe
        {
            fixed (byte* pSrc = srcData)
            {
                source.CopyPixels(
                    new PixelRect(0, 0, width, height),
                    (nint)pSrc,
                    srcData.Length,
                    stride
                );
            }

            var resultData = new byte[totalBytes];
            var angleStep = (float)(2.0 * Math.PI / Math.Max(1, samples));
            var contrastFactor = (1.0f + contrast) * (1.0f + contrast);

            Parallel.For(
                0,
                height,
                y =>
                {
                    var rowOffset = y * stride;

                    for (var x = 0; x < width; x++)
                    {
                        var offset = rowOffset + x * 4;

                        var b = srcData[offset];
                        var g = srcData[offset + 1];
                        var r = srcData[offset + 2];
                        var a = srcData[offset + 3];
                        var srcLuma = GetGrayscale(r, g, b, mode);
                        var totalSampleLuma = 0f;
                        var validSamples = 0;

                        for (var s = 0; s < samples; s++)
                        {
                            var angle = s * angleStep;

                            for (var it = 1; it <= iterations; it++)
                            {
                                var currentRadius = radius / (float)iterations * it;

                                var sampleX = Math.Clamp(
                                    (int)(x + Math.Cos(angle) * currentRadius),
                                    0,
                                    width - 1
                                );
                                var sampleY = Math.Clamp(
                                    (int)(y + Math.Sin(angle) * currentRadius),
                                    0,
                                    height - 1
                                );

                                var sampleOffset = sampleY * stride + sampleX * 4;
                                var sb = srcData[sampleOffset];
                                var sg = srcData[sampleOffset + 1];
                                var sr = srcData[sampleOffset + 2];
                                totalSampleLuma += GetGrayscale(sr, sg, sb, mode);
                                validSamples++;
                            }
                        }

                        var localBgLuma =
                            validSamples > 0 ? totalSampleLuma / validSamples : srcLuma;

                        // 2. Локальна дельта контрасту
                        var delta = srcLuma - localBgLuma;
                        var baseGray = 128.0f + delta * 2.0f; // Підсилений коефіцієнт дельти (2.0 замість 1.8)

                        // 3. Контрастність
                        var gray = (baseGray - 128.0f) * contrastFactor + 128.0f;

                        // 4. Очищення паперу (Levels / Thresholding для документів)
                        if (gray >= whitePoint)
                        {
                            gray = 255.0f; // Папір робимо повністю білим
                        }
                        else if (gray <= blackPoint)
                        {
                            gray = 0.0f; // Текст/лінії робимо глибоко чорними
                        }
                        else
                        {
                            // Лінійна нормалізація діапазону [blackPoint .. whitePoint] у [0 .. 255]
                            gray = (gray - blackPoint) / (whitePoint - blackPoint) * 255.0f;
                        }

                        var grayByte = (byte)Math.Clamp(gray, 0.0f, 255.0f);

                        resultData[offset] = grayByte; // B
                        resultData[offset + 1] = grayByte; // G
                        resultData[offset + 2] = grayByte; // R
                        resultData[offset + 3] = a; // A
                    }
                }
            );

            var resultBitmap = new WriteableBitmap(
                pixelSize,
                source.Dpi,
                PixelFormat.Bgra8888,
                AlphaFormat.Premul
            );

            using (var fb = resultBitmap.Lock())
            {
                var dstPtr = (byte*)fb.Address;
                var rowBytes = fb.RowBytes;

                fixed (byte* resPtr = resultData)
                {
                    for (var y = 0; y < height; y++)
                    {
                        Unsafe.CopyBlock(dstPtr + y * rowBytes, resPtr + y * stride, (uint)stride);
                    }
                }
            }

            return resultBitmap;
        }
    }

    private static float GetGrayscale(byte r, byte g, byte b, GrayscaleMode mode)
    {
        return mode switch
        {
            GrayscaleMode.Luma => 0.299f * r + 0.587f * g + 0.114f * b,
            GrayscaleMode.Luminance => 0.2126f * r + 0.7152f * g + 0.0722f * b,
            GrayscaleMode.Lightness => (Math.Max(r, Math.Max(g, b)) + Math.Min(r, Math.Min(g, b)))
                / 2f,
            GrayscaleMode.Average => (r + g + b) / 3f,
            GrayscaleMode.Value => Math.Max(r, Math.Max(g, b)),
            _ => 0.22f * r + 0.72f * g + 0.06f * b,
        };
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
