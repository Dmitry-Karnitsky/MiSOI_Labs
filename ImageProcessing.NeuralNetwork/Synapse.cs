using System;

namespace Levshits.NeuralNetwork
{
    [Serializable]
    public class Synapse
    {
        public Neuron Neuron { get; set; }
        public double Weight { get; set; }
    }
}