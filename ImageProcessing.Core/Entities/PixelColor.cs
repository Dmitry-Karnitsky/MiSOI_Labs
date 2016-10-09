namespace ImageProcessing.Core.Entities
{
    public class PixelColor
    {
        private byte _r;
        private byte _g;
        private byte _b;

        public int B
        {
            get { return _b; }
            set
            {
                _b = GetColorValue(value);
                IsChanged = true;
            }
        }

        public int G
        {
            get { return _g; }
            set
            {
                _g = GetColorValue(value);
                IsChanged = true;
            }
        }

        public int R
        {
            get { return _r; }
            set
            {
                _r = GetColorValue(value);
                IsChanged = true;
            }
        }

        public bool IsChanged { get; set; }

        private static byte GetColorValue(int value)
        {
            if (value > 0)
            {
                return (byte)(value > 255 ? 255 : value);
            }

            return 0;
        }
    }
}
