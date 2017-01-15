using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Genetic.Params {

    public class ConstantParamsProvider {
        public GeneticParams Params { get; }

        public ConstantParamsProvider(float eletismFraction, float mutationFraction) {
            Params = new GeneticParams(eletismFraction, mutationFraction);
        }

        public GeneticParams GetParams(Genetic _) => Params;
    }
}