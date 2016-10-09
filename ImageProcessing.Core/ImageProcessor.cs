using System.Drawing;
using System.Threading.Tasks;

namespace ImageProcessing.Core
{
    public class ImageProcessor
    {
        public Bitmap OriginalImage { get; set; }

        public Bitmap ProcessedImage { get; protected set; }

        public virtual Task<Bitmap> Process()
        {
            var task = DefaultResult();
            ProcessedImage = task.Result;

            return task;
        }

        public async Task<Bitmap> AdjustBrightness(int value)
        {
            if (ProcessedImage == null) return null;

            var clone = (Bitmap)ProcessedImage.Clone();

            return await Task.Run(() => clone.ForEachPixel(
                pixel =>
                {
                    pixel.B += value;
                    pixel.G += value;
                    pixel.R += value;
                }));
        }

        protected Task<Bitmap> DefaultResult()
        {
            return Task.FromResult(OriginalImage);
        }
    }
}
