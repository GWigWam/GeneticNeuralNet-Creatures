using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Genetic.Params {

    public struct GeneticParams {
        public float ElitismFraction { get; }
        public float MutationFraction { get; }

        public GeneticParams(float eletismFraction, float mutationFraction) {
            ElitismFraction = eletismFraction;
            MutationFraction = mutationFraction;
        }
    }
}