using System;

namespace ImageProcessing.NeuralNetwork
{
    [Serializable]
    public class Synapse
    {
        public Neuron Neuron { get; set; }
        public double Weight { get; set; }
    }
}