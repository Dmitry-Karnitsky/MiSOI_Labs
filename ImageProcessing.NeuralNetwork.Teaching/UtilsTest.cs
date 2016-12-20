using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageProcessing.NeuralNetwork.Teaching
{
    [TestClass]
    public class UtilsTest
    {
       [TestMethod]
        public void PrepareTestFiles()
        {
            int magicNumber;
            int numberOrImages;
            int numberOrRows;
            int numberOrCols;

            byte[] intBuffer = new byte[4];

            using (FileStream fs = File.OpenRead("../../Resources/train-images.idx3-ubyte"))
            {
                fs.Read(intBuffer, 0, 4);
                intBuffer = intBuffer.Reverse().ToArray();
                magicNumber = BitConverter.ToInt32(intBuffer, 0);
                fs.Read(intBuffer, 0, 4);
                intBuffer = intBuffer.Reverse().ToArray();
                numberOrImages = BitConverter.ToInt32(intBuffer, 0);
                fs.Read(intBuffer, 0, 4);
                intBuffer = intBuffer.Reverse().ToArray();
                numberOrRows = BitConverter.ToInt32(intBuffer, 0);
                fs.Read(intBuffer, 0, 4);
                intBuffer = intBuffer.Reverse().ToArray();
                numberOrCols = BitConverter.ToInt32(intBuffer, 0);

                byte[] imageBufferBytes = new byte[numberOrRows*numberOrCols];
                for (int i = 0; i < 1000; i++)
                {
                    fs.Read(imageBufferBytes, 0, imageBufferBytes.Length);

                    using (var bitmap = new Bitmap(numberOrCols, numberOrRows, PixelFormat.Format8bppIndexed))
                    {
                        imageBufferBytes = imageBufferBytes.Select(x => (byte)(255 - x)).ToArray();
                        var boundsRect = new Rectangle(0, 0, numberOrCols, numberOrRows);
                        BitmapData bmpData = bitmap.LockBits(boundsRect,
                            ImageLockMode.ReadWrite,
                            bitmap.PixelFormat);
                        IntPtr ptr = bmpData.Scan0;
                        Marshal.Copy(imageBufferBytes, 0, ptr, imageBufferBytes.Length);
                        bitmap.UnlockBits(bmpData);
                        if (File.Exists($"../../Resources/Images/{i}.bmp"))
                        {
                            File.Delete($"../../Resources/Images/{i}.bmp");
                        }

                        bitmap.Save($"../../Resources/Images/{i}.bmp");
                    }
                }
            }

            Assert.AreEqual(28, numberOrCols);
            Assert.AreEqual(28, numberOrRows);
        }
    }
}
