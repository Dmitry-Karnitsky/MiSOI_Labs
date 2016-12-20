using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace Levshits.NeuralNetwork.Utils
{
    public static class NeuralNetworkUtils
    {
        public static void SaveNetwork(this Network network, string path)
        {
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                System.Runtime.Serialization.IFormatter formatter =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(stream, network);
            }
        }

        public static Network LoadNetwork(string path)
        {
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                System.Runtime.Serialization.IFormatter formatter =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                Network ob = (Network) formatter.Deserialize(stream);
                stream.Close();
                return ob;
            }
        }

        public static Network CreateNeuralNetwork(int width, int height, int symbolsCount)
        {
            return Network.GetInstance(width*height, symbolsCount);
        }
    }
}