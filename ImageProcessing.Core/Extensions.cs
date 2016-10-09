using System;
using System.Drawing;
using System.Drawing.Imaging;
using ImageProcessing.Core.Entities;

namespace ImageProcessing.Core
{
    public static class Extensions
    {
        public static Bitmap SafeLockBits(this Bitmap bitmap, ImageLockMode lockMode, Action<BitmapData> bitmapDataProcessor)
        {
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(rect, lockMode, bitmap.PixelFormat);

            try
            {
                bitmapDataProcessor(bitmapData);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }

        public static Bitmap ApplyMatrix3X3(this Bitmap bitmap, int[,] matrix)
        {
            bitmap.SafeLockBits(ImageLockMode.ReadWrite, bitmapData =>
            {
                var bitmapClone = (Bitmap)bitmap.Clone();

                bitmapClone.SafeLockBits(ImageLockMode.ReadOnly, bitmapDataSrc =>
                {
                    var bytesPerPixel = Image.GetPixelFormatSize(bitmapDataSrc.PixelFormat) / 8;

                    var stride = bitmapData.Stride;

                    unsafe
                    {
                        var ptrPixels = (byte*)(void*)bitmapData.Scan0;
                        var ptrPixelsSrc = (byte*)(void*)bitmapDataSrc.Scan0;

                        var offset = stride - bitmap.Width * bytesPerPixel;
                        var width = bitmap.Width - 2;
                        var height = bitmap.Height - 2;
                        var currentPixel = stride + bytesPerPixel;

                        for (var y = 0; y < height; y++)
                        {
                            for (var x = 0; x < width; x++)
                            {
                                for (var i = 0; i < 3; i++)
                                {
                                    var modifiedPixelColor = ApplyMatrixOnColor(ptrPixelsSrc, matrix, i, bytesPerPixel, stride);

                                    ptrPixels[currentPixel + i] = (byte)modifiedPixelColor;
                                }

                                ptrPixels += bytesPerPixel;
                                ptrPixelsSrc += bytesPerPixel;
                            }

                            ptrPixels += offset;
                            ptrPixelsSrc += offset;
                        }
                    }
                });
            });

            return bitmap;
        }

        public static Bitmap ModifyColorsUsingBitmap(this Bitmap bitmapToModify, Bitmap sourceBitmap, Func<PixelColor, PixelColor, PixelColor> colorsMergeFunc)
        {
            bitmapToModify.SafeLockBits(ImageLockMode.ReadWrite, bitmapData =>
            {
                sourceBitmap.SafeLockBits(ImageLockMode.ReadOnly, bitmapDataSrc =>
                {
                    var bytesPerPixel = Image.GetPixelFormatSize(bitmapDataSrc.PixelFormat) / 8;
                    var stride = bitmapData.Stride;

                    unsafe
                    {
                        var ptrPixels = (byte*)(void*)bitmapData.Scan0;
                        var ptrPixelsSrc = (byte*)(void*)bitmapDataSrc.Scan0;

                        var offset = stride - bitmapToModify.Width * bytesPerPixel;

                        var color1 = new PixelColor();
                        var color2 = new PixelColor();

                        for (var y = 0; y < bitmapToModify.Height; y++)
                        {
                            for (var x = 0; x < bitmapToModify.Width; x++)
                            {
                                color1.FillTransportColor(ptrPixels);
                                color2.FillTransportColor(ptrPixelsSrc);

                                colorsMergeFunc(color1, color2).FillBitmapColor(ptrPixels);

                                ptrPixels += bytesPerPixel;
                                ptrPixelsSrc += bytesPerPixel;
                            }

                            ptrPixels += offset;
                            ptrPixelsSrc += offset;
                        }
                    }
                });
            });

            return bitmapToModify;
        }

        public static Bitmap ForEachPixel(this Bitmap bitmapToModify, Action<PixelColor> colorModifier)
        {
            bitmapToModify.SafeLockBits(ImageLockMode.ReadWrite, bitmapData =>
            {
                var bytesPerPixel = Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
                var stride = bitmapData.Stride;

                unsafe
                {
                    var ptrPixels = (byte*)(void*)bitmapData.Scan0;

                    var offset = stride - bitmapToModify.Width * bytesPerPixel;

                    var color1 = new PixelColor();

                    for (var y = 0; y < bitmapToModify.Height; y++)
                    {
                        for (var x = 0; x < bitmapToModify.Width; x++)
                        {
                            color1.FillTransportColor(ptrPixels)
                                .Modify(colorModifier)
                                .FillBitmapColor(ptrPixels);

                            ptrPixels += bytesPerPixel;
                        }

                        ptrPixels += offset;
                    }
                }
            });

            return bitmapToModify;
        }

        public static PixelColor Grayscale(this PixelColor pixel)
        {
            pixel.R = pixel.G = pixel.B = (byte)(pixel.R * .299 + pixel.G * .587 + pixel.B * 0.114);

            return pixel;
        }

        public static PixelColor ForEachColor(this PixelColor pixel, Func<int, int> func)
        {
            pixel.B = func(pixel.B);
            pixel.G = func(pixel.G);
            pixel.R = func(pixel.R);

            return pixel;
        }

        private static unsafe int ApplyMatrixOnColor(byte* ptr, int[,] matrix, int colorOffset, int bytesPerPixel, int stride)
        {
            var stride2 = stride * 2;

            var firstPixelOffset = colorOffset;
            var secondPixelOffset = bytesPerPixel + colorOffset;
            var thirdPixelOffset = bytesPerPixel * 2 + colorOffset;

            var pixelColor =
                ptr[firstPixelOffset] * matrix[0, 0] + ptr[secondPixelOffset] * matrix[0, 1] + ptr[thirdPixelOffset] * matrix[0, 2] +
                ptr[firstPixelOffset + stride] * matrix[1, 0] + ptr[secondPixelOffset + stride] * matrix[1, 1] + ptr[thirdPixelOffset + stride] * matrix[1, 2] +
                ptr[firstPixelOffset + stride2] * matrix[2, 0] + ptr[secondPixelOffset + stride2] * matrix[2, 1] + ptr[thirdPixelOffset + stride2] * matrix[2, 2];

            if (pixelColor < 0) pixelColor = 0;
            if (pixelColor > 255) pixelColor = 255;

            return pixelColor;
        }

        private static unsafe PixelColor FillTransportColor(this PixelColor c, byte* ptr)
        {
            c.B = ptr[0];
            c.G = ptr[1];
            c.R = ptr[2];

            return c;
        }

        private static unsafe void FillBitmapColor(this PixelColor c, byte* ptr)
        {
            ptr[0] = (byte)c.B;
            ptr[1] = (byte)c.G;
            ptr[2] = (byte)c.R;
        }

        private static PixelColor Modify(this PixelColor color, Action<PixelColor> modifier)
        {
            modifier(color);
            return color;
        }
    }
}
