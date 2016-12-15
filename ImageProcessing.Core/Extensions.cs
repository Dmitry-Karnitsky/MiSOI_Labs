using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
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

        public static Bitmap MedianFilter(this Bitmap bitmapToModify, Bitmap sourceBitmap, int size)
        {
            bitmapToModify.SafeLockBits(ImageLockMode.ReadWrite, bitmapData =>
            {
                int bytesPerPixel = Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                int shift = (size / 2 + size % 2);

                sourceBitmap.SafeLockBits(ImageLockMode.ReadWrite, sourceBitmapData =>
                {
                    unsafe
                    {
                        byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                        byte* PtrFirstFilterPixel = (byte*)sourceBitmapData.Scan0;

                        Parallel.For((long)(shift), heightInPixels - shift,
                            new ParallelOptions { MaxDegreeOfParallelism = 16 },
                            y =>
                            {
                                byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                                for (int x = shift * bytesPerPixel; x < widthInBytes - shift * bytesPerPixel; x = x + bytesPerPixel)
                                {
                                    List<byte> items = new List<byte>();

                                    for (long filtery = y - shift; filtery < y + shift; filtery++)
                                    {
                                        byte* currentFilterLine = PtrFirstFilterPixel + (filtery * sourceBitmapData.Stride);
                                        for (int filterx = x - shift * bytesPerPixel; filterx < x + shift * bytesPerPixel; filterx = filterx + bytesPerPixel)
                                        {
                                            items.Add((byte)Math.Truncate(0.299 * currentFilterLine[filterx + 2] + 0.587 * currentFilterLine[filterx + 1] + 0.114 * currentFilterLine[filterx]));
                                        }
                                    }
                                    currentLine[x] = items.OrderBy(item => item).ToArray()[items.Count / 2];
                                    currentLine[x + 1] = items.OrderBy(item => item).ToArray()[items.Count / 2];
                                    currentLine[x + 2] = items.OrderBy(item => item).ToArray()[items.Count / 2];
                                }
                            });
                    }
                });
            });

            return bitmapToModify;
        }

        public static List<Point> ApplyForstnerDetector(this Bitmap bitmapToModify, double sigma, int windowSize)
        {
            List<Point> cornersList = new List<Point>();

            bitmapToModify.SafeLockBits(ImageLockMode.ReadWrite, bitmapData =>
            {
                int bytesPerPixel = Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
                int height = bitmapData.Height;
                int width = bitmapData.Width;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                int stride = bitmapData.Stride;
                int offset = stride - widthInBytes;


                // 1. Calculate partial differences
                float[,] diffx = new float[height, width];
                float[,] diffy = new float[height, width];
                float[,] diffxy = new float[height, width];

                unsafe
                {
                    fixed (float* pdx = diffx, pdy = diffy, pdxy = diffxy)
                    {
                        // Begin skipping first line
                        byte* src = (byte*)bitmapData.Scan0 + stride;
                        float* dx = pdx + width;
                        float* dy = pdy + width;
                        float* dxy = pdxy + width;

                        // for each line
                        for (int y = 1; y < height - 1; y++)
                        {
                            // skip first column
                            dx++; dy++; dxy++; src += bytesPerPixel;

                            // for each inner pixel in line (skipping first and last)
                            for (int x = 1; x < width - 1; x++, src += bytesPerPixel, dx++, dy++, dxy++)
                            {
                                // Retrieve the pixel neighborhood
                                byte a11 = src[+stride + bytesPerPixel], a12 = src[+bytesPerPixel], a13 = src[-stride + bytesPerPixel];
                                byte a21 = src[+stride + 0], /*  a22    */  a23 = src[-stride + 0];
                                byte a31 = src[+stride - bytesPerPixel], a32 = src[-bytesPerPixel], a33 = src[-stride - bytesPerPixel];

                                // Convolution with horizontal differentiation kernel mask
                                float h = ((a11 + a12 + a13) - (a31 + a32 + a33)) * 0.166666667f;

                                // Convolution with vertical differentiation kernel mask
                                float v = ((a11 + a21 + a31) - (a13 + a23 + a33)) * 0.166666667f;

                                // Store squared differences directly
                                *dx = h * h;
                                *dy = v * v;
                                *dxy = h * v;
                            }

                            // Skip last column
                            dx++; dy++; dxy++;
                            src += offset + bytesPerPixel;
                        }

                    }
                }
                float[] kernel = Kernel(sigma * sigma, 7);
                if (sigma > 0.0)
                {
                    float[,] temp = new float[height, width];
                    Convolve(diffx, temp, kernel);
                    Convolve(diffy, temp, kernel);
                    Convolve(diffxy, temp, kernel);
                }
                // 3. Compute Harris Corner Response Map
                float[,] map = new float[height, width];

                unsafe
                {
                    fixed (float* pdx = diffx, pdy = diffy, pdxy = diffxy, pmap = map)
                    {
                        float* dx = pdx;
                        float* dy = pdy;
                        float* dxy = pdxy;
                        float* H = pmap;
                        float M, A, B, C, O;

                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++, dx++, dy++, dxy++, H++)
                            {
                                A = *dx;
                                B = *dy;
                                C = *dxy;

                                M = (float)((A * B - C * C) / (A + B));
                                O = (float)(4 * (A * B - C * C) / ((A + B) * (A + B)));

                                if (M > 0.5 && !float.IsNaN(M) && O > 0.5 && !float.IsNaN(O))
                                {
                                    *H = M; // insert value in the map
                                }
                            }
                        }
                    }
                }


                // 4. Suppress non-maximum points
                for (int y = windowSize, maxY = height - windowSize; y < maxY; y++)
                {
                    for (int x = windowSize, maxX = width - windowSize; x < maxX; x++)
                    {
                        float currentValue = map[y, x];

                        for (int i = -windowSize; (Math.Abs(currentValue) > 1) && (i <= windowSize); i++)
                        {
                            for (int j = -windowSize; j <= windowSize; j++)
                            {
                                if (map[y + i, x + j] > currentValue)
                                {
                                    currentValue = 0;
                                    break;
                                }
                            }
                        }

                        if (Math.Abs(currentValue) > 1)
                        {
                            cornersList.Add(new Point(x, y));
                        }
                    }
                }
            });

            return cornersList;
        }

        public static Bitmap MarkAreas(this Bitmap bitmap, List<Point> points)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                foreach (var point in points)
                {
                    graphics.DrawRectangle(Pens.DarkBlue, point.X - 5, point.Y - 5, 10, 10);
                }
            }

            return bitmap;
        }

        public static PixelColor Grayscale(this PixelColor pixel)
        {
            pixel.R = pixel.G = pixel.B = (byte)(pixel.R * .299 + pixel.G * .587 + pixel.B * 0.114);

            return pixel;
        }

        public static byte GetBrightness(this PixelColor pixel)
        {
            var brightness = .2126 * pixel.R + .7152 * pixel.G + .0722 * pixel.B;

            return (byte)(brightness > 255 ? 255 : brightness);
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
            c.IsChanged = false;

            return c;
        }

        private static unsafe void FillBitmapColor(this PixelColor c, byte* ptr)
        {
            if (!c.IsChanged) return;

            ptr[0] = (byte)c.B;
            ptr[1] = (byte)c.G;
            ptr[2] = (byte)c.R;
        }

        private static PixelColor Modify(this PixelColor color, Action<PixelColor> modifier)
        {
            modifier(color);
            return color;
        }

        private static void Convolve(float[,] image, float[,] temp, float[] kernel)
        {
            int width = image.GetLength(1);
            int height = image.GetLength(0);
            int radius = kernel.Length / 2;

            unsafe
            {
                fixed (float* ptrImage = image, ptrTemp = temp)
                {
                    float* src = ptrImage + radius;
                    float* tmp = ptrTemp + radius;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = radius; x < width - radius; x++, src++, tmp++)
                        {
                            float v = 0;
                            for (int k = 0; k < kernel.Length; k++)
                                v += src[k - radius] * kernel[k];
                            *tmp = v;
                        }
                        src += 2 * radius;
                        tmp += 2 * radius;
                    }


                    for (int x = 0; x < width; x++)
                    {
                        for (int y = radius; y < height - radius; y++)
                        {
                            src = ptrImage + y * width + x;
                            tmp = ptrTemp + y * width + x;

                            float v = 0;
                            for (int k = 0; k < kernel.Length; k++)
                                v += tmp[width * (k - radius)] * kernel[k];
                            *src = v;
                        }
                    }
                }
            }
        }

        private static float[] Kernel(double sigmaSquared, int size)
        {
            if (((size % 2) == 0) || (size < 3))
                throw new ArgumentOutOfRangeException("size", "Kernel size must be odd and higher than 2.");

            int r = size / 2;

            float[] kernel = new float[size];
            for (int x = -r, i = 0; i < size; x++, i++)
                kernel[i] = Gaussian(sigmaSquared, x);

            return kernel;
        }

        private static float Gaussian(double sigmaSquared, int x)
        {
            return (float)(Math.Exp(x * x / (-2 * sigmaSquared)) / (Math.Sqrt(2 * Math.PI * sigmaSquared)));
        }
    }
}
