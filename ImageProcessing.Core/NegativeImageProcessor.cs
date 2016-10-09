using System.Drawing;
using System.Threading.Tasks;

namespace ImageProcessing.Core
{
    public class NegativeImageProcessor : ImageProcessor
    {
        public override async Task<Bitmap> Process()
        {
            if (OriginalImage == null)
            {
                return await DefaultResult();
            }

            var clone = (Bitmap)OriginalImage.Clone();

            ProcessedImage = await Task.Run(() => clone.ForEachPixel(pixel => pixel.ForEachColor(color => 255 - color)));

            return ProcessedImage;
        }
    }
}
