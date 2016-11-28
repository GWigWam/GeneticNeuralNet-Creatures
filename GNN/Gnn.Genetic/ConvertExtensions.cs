using Gnn.NeuralNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Genetic {

    public static class ConvertExtensions {

        public static Individual ToIndividual(this Network network, float fitness) {
            return new Individual(network.Weights.Select(w => w.Value).ToArray(), fitness);
        }

        public static Network SetWeights(this Network network, Individual source) {
            for(int i = 0; i < source.Values.Length; i++) {
                network.Weights[i].Value = source.Values[i];
            }
            return network;
        }
    }
}