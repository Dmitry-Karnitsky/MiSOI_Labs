using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Levshits.NeuralNetwork.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Levshits.NeuralNetwork.Testing
{
    [TestClass]
    public class TrainingTest
    {
        private const int NumInputs = 128;
        private const int NumHiddenPerLayer = 10;
        private const int NumHiddenLayers = 1;
        private const int NumOutputs = 10;
        private const string NetworkJsonPath = @"../../Resources/network.dat";

        [TestMethod]
        public void TrainNetwork()
        {
            var network = CreateNetwork();
            for (int i = 0; i < 50; i++)
            {
                RunTrainning((bytes) =>
                {
                    var value = network.Train(bytes.Select(x => (double) x).ToArray(), 0.0005);
                }, 20);
            }
            network.SaveNetwork(NetworkJsonPath);

            Dictionary<int, int> groups = new Dictionary<int, int>();
            int index = 0;
            RunTrainning((bytes) =>
            {
                
                var value = network.Process(bytes.Select(x => (double)x).ToArray()).ToList();
                var max = value.Max();
                groups[index] = value.IndexOf(max); 
                index++;

            }, 10);
            network = NeuralNetworkUtils.LoadNetwork(NetworkJsonPath);
            groups = new Dictionary<int, int>();
            index = 0;
            RunTrainning((bytes) =>
            {

                var value = network.Process(bytes.Select(x => (double)x).ToArray()).ToList();
                var max = value.Max();
                groups[index] = value.IndexOf(max);
                index++;

            }, 10);
            Assert.AreEqual(10, groups.Count);
        }

        private static Network CreateNetwork()
        {
            var network = NeuralNetworkUtils.CreateNeuralNetwork(28, 28, 10);
            return network;
        }

        private static void RunTrainning(Action<byte[]> action, int maxCount)
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

                byte[] imageBufferBytes = new byte[numberOrRows * numberOrCols];
                for (int i = 0; i < Math.Min(maxCount, numberOrImages); i++)
                {
                    fs.Read(imageBufferBytes, 0, imageBufferBytes.Length);

                    imageBufferBytes = imageBufferBytes.Select(x => (byte)(255 - x)).ToArray();

                    action(imageBufferBytes);
                }
            }
        }
    }
}
