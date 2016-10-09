using System;
using System.Drawing;
using System.Threading.Tasks;
using ImageProcessing.Core.Entities;

namespace ImageProcessing.Core
{
    public class SobelEdgeProcessor : ImageProcessor
    {
        private readonly int[,] _matrix1 =
        {
            {1,  0, -1},
            {2,  0, -2},
            {1,  0, -1}
        };

        private readonly int[,] _matrix2 =
        {
            {-1, -2, -1},
            { 0,  0,  0},
            { 1,  2,  1}
        };

        public override async Task<Bitmap> Process()
        {
            if (OriginalImage == null)
            {
                return await DefaultResult();
            }

            var clone1 = (Bitmap)OriginalImage.Clone();
            var clone2 = (Bitmap)OriginalImage.Clone();

            await Task.Run(() => clone1.ApplyMatrix3X3(_matrix1));
            await Task.Run(() => clone2.ApplyMatrix3X3(_matrix2));

            ProcessedImage =  await Task.Run(() => clone1.ModifyColorsUsingBitmap(clone2,
                (color1, color2) => MergeFirstColorWithSecond(color1, color2).Grayscale()));

            return ProcessedImage;
        }

        private static PixelColor MergeFirstColorWithSecond(PixelColor c1, PixelColor c2)
        {
            c1.B = CalculatePower(c1.B, c2.B);
            c1.G = CalculatePower(c1.G, c2.G);
            c1.R = CalculatePower(c1.R, c2.R);

            ApplyThreshold(c1);

            return c1;
        }

        private static int CalculatePower(int c1, int c2)
        {
            return (int)Math.Sqrt(c1 * c1 + c2 * c2);
        }

        private static void ApplyThreshold(PixelColor c)
        {
            const byte threshold = 127;

            if (c.B > threshold || c.G > threshold || c.R > threshold)
            {
                c.B = 255;
                c.G = 255;
                c.R = 255;
            }
        }
    }
}
