using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageProcessing.Core
{
    public class NegativeImageProcessor : ImageProcessor
    {
        private const int RedColor = 2;
        private const int GreenColor = 1;
        private const int BlueColor = 0;

        public override async Task<Bitmap> Process()
        {
            if (OriginalImage == null)
            {
                return await DefaultResult();
            }

            var clone = (Bitmap)OriginalImage.Clone();

            ProcessedImage = await Task.Run(() => clone.ForEachPixel(pixel => pixel.ForEachColor(color => 255 - color)));

            //ProcessedImage = await Task.Run(() => clone.SafeLockBits(ImageLockMode.ReadWrite,
            //    bitmapData =>
            //    {
            //        unsafe
            //        {
            //            var bytesPerPixel = Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
            //            var heightInPixels = bitmapData.Height;
            //            var widthInBytes = bitmapData.Width * bytesPerPixel;
            //            var ptrFirstPixel = (byte*)bitmapData.Scan0;

            //            Parallel.For(0, heightInPixels, y =>
            //            {
            //                var currentLine = ptrFirstPixel + y * bitmapData.Stride;
            //                for (var x = 0; x < widthInBytes; x = x + bytesPerPixel)
            //                {
            //                    currentLine[x + BlueColor] = (byte)(255 - currentLine[x + BlueColor]);
            //                    currentLine[x + GreenColor] = (byte)(255 - currentLine[x + GreenColor]);
            //                    currentLine[x + RedColor] = (byte)(255 - currentLine[x + RedColor]);
            //                }
            //            });
            //        }
            //    }));

            return ProcessedImage;
        }
    }
}
