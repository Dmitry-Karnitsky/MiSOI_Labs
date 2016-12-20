using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ImageProcessing.NeuralNetwork
{
    [Serializable]
    public class Network
    {
        private Network()
        {
            InputNeurons = new List<Neuron>();
            OutputNeurons = new List<Neuron>();
            Wins = new Dictionary<Neuron, int>();
        }
        public List<Neuron> InputNeurons { get; set; }
        public List<Neuron> OutputNeurons { get; set; }
        [JsonIgnore]
        public Dictionary<Neuron, int> Wins { get; set; }

        public static Network GetInstance(int inputs, int outputs)
        {
            var network = new Network();
            network.Initialize(inputs, outputs);
            return network;
        }

        private void Initialize(int inputs, int outputs)
        {
            var random = new Random();
            for (int i = 0; i < inputs; i++)
            {
                InputNeurons.Add(new Neuron());
            }
            for (int i = 0; i < outputs; i++)
            {
                var dentrites = new List<Synapse>();
                foreach (var input in InputNeurons)
                {
                    dentrites.Add(new Synapse() {Neuron = input, Weight = (random.NextDouble() - 0.5)*0.01});
                }
                OutputNeurons.Add(new Neuron() {Dendrites =  dentrites});
            }
        }

        public double[] Process(double[] inputs)
        {
            if (inputs.Length != InputNeurons.Count)
            {
                throw new ArgumentException();
            }
            for (int i = 0; i < InputNeurons.Count; i++)
            {
                InputNeurons[i].Value = inputs[i];
            }
            foreach (var outputNeuron in OutputNeurons)
            {
                outputNeuron.Calculate();
            }
            return OutputNeurons.Select(x => x.Value).ToArray();
        }

        public double[] Train(double[] inputs, double trainSpeed = 0.05)
        {
            if (inputs.Length != InputNeurons.Count)
            {
                throw new ArgumentException();
            }
            for (int i = 0; i < InputNeurons.Count; i++)
            {
                InputNeurons[i].Value = inputs[i];
            }
            Parallel.ForEach(OutputNeurons, x => x.Calculate());
            var winner =
                OutputNeurons.Select(
                    x =>
                        new
                        {
                            Neuron = x,
                            Value =
                            x.Dendrites.Sum(item => Math.Abs(item.Neuron.Value - item.Weight))*
                            (Wins.ContainsKey(x) ? Wins[x] : 1)
                        })
                        .OrderBy(x=>x.Value)
                        .First();

            Wins[winner.Neuron] = Wins.ContainsKey(winner.Neuron) ? Wins[winner.Neuron] + 1 : 2;

            foreach (var dendrite in winner.Neuron.Dendrites)
            {
                dendrite.Weight = dendrite.Weight + trainSpeed*(dendrite.Neuron.Value - dendrite.Weight);
            }

            return OutputNeurons.Select(x => x.Value).ToArray();
        }
    }
}