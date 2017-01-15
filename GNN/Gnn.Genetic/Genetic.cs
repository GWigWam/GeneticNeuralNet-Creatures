using Gnn.Genetic.Params;
using Gnn.Genetic.Procedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Genetic {

    public class Genetic {
        public GeneticParamsProvider ParamsProvider { get; set; }

        public Func<float> RandomValueProvider { get; set; }

        public Genetic(GeneticParamsProvider paramsProvider, Func<float> randomValueProvider) {
            ParamsProvider = paramsProvider;
            RandomValueProvider = randomValueProvider;
        }

        public IEnumerable<Individual> Apply(IEnumerable<Individual> source) {
            var prms = ParamsProvider(this);
            var ar = source.ToArray();

            var elite = Elitism.SelectElite(ar, prms.ElitismFraction).ToArray();
            if(elite.Length % 2 != 0) {
                throw new InvalidOperationException("Elitism fraction must result in an even number of individuals");
            }
            var selected = Selection.Roulette(ar, ar.Length - elite.Length);
            var crossed = Crossover.UniformCrossoverPop(selected);
            var mutated = Mutation.MutatePop(crossed, prms.MutationFraction, RandomValueProvider);

            return elite.Concat(mutated);
        }
    }
}