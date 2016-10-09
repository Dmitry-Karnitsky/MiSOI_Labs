using System.Drawing;
using System.Threading.Tasks;

namespace ImageProcessing.Core
{
    public class ImageProcessor
    {
        private Bitmap _processedImage;

        public Bitmap OriginalImage { get; set; }

        public Bitmap ProcessedImage
        {
            get { return _processedImage; }
            protected set
            {
                _processedImage = value;
                AdjustedImage = (Bitmap)value.Clone();
            }
        }

        public Bitmap AdjustedImage { get; private set; }

        public virtual Task<Bitmap> Process()
        {
            var task = DefaultResult();
            ProcessedImage = task.Result;

            return task;
        }

        public async Task<Bitmap> AdjustBrightness(int value)
        {
            if (ProcessedImage == null)
            {
                return await DefaultResult();
            }

            var clone = (Bitmap)ProcessedImage.Clone();

            AdjustedImage = await Task.Run(() => clone.ForEachPixel(
                pixel =>
                {
                    pixel.B += value;
                    pixel.G += value;
                    pixel.R += value;
                }));

            return AdjustedImage;
        }

        public async Task<int[]> GetBrightnessHistogramValues()
        {
            var result = new int[256];

            if (AdjustedImage == null)
            {
                return await Task.FromResult(result);
            }

            var clone = (Bitmap)AdjustedImage.Clone();

            return await Task.Run(() => clone.ForEachPixel(
                    pixel =>
                    {
                        result[pixel.GetBrightness()]++;
                    }))
                    .ContinueWith(bitmap => result);
        }

        protected Task<Bitmap> DefaultResult()
        {
            return Task.FromResult(OriginalImage);
        }
    }
}
