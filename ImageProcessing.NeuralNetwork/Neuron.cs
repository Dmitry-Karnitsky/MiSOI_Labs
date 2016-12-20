using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageProcessing.NeuralNetwork
{
    [Serializable]
    public class Neuron
    {
        public List<Synapse> Dendrites { get; set; }
        public double Value { get; set; }

        public void Calculate()
        {
            Value = Dendrites.Sum(x => x.Weight*x.Neuron.Value);
        }
    }
}