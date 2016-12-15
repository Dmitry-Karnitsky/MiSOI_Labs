using System.Drawing;
using System.Threading.Tasks;

namespace ImageProcessing.Core
{
    public class ForstnerDetectorProcessor : ImageProcessor
    {
        public override async Task<Bitmap> Process()
        {
            if (OriginalImage == null)
            {
                return await DefaultResult();
            }

            var bitmap = (Bitmap)OriginalImage.Clone();

            return await Task.Run(() => bitmap.ForEachPixel(p => p.Grayscale()))
                 .ContinueWith(task => task.Result.MedianFilter((Bitmap)bitmap.Clone(), 3))
                 .ContinueWith(task => task.Result.ApplyForstnerDetector(2, 4))
                 .ContinueWith(task => bitmap.MarkAreas(task.Result));
        }
    }
}
