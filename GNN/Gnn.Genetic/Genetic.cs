using Gnn.Genetic.Procedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Genetic {

    public class Genetic {
        public float ElitismFraction { get; set; }
        public float MutationFraction { get; set; }
        public Func<float> RandomValueProvider { get; set; }

        public Genetic(float elitismFraction, float mutationFraction, Func<float> randomValueProvider) {
            ElitismFraction = elitismFraction;
            MutationFraction = mutationFraction;
            RandomValueProvider = randomValueProvider;
        }

        public IEnumerable<Individual> Apply(IEnumerable<Individual> source) {
            var ar = source.ToArray();

            var elite = Elitism.SelectElite(ar, ElitismFraction).ToArray();
            if(elite.Length % 2 != 0) {
                throw new InvalidOperationException("Elitism fraction must result in an even number of individuals");
            }
            var selected = Selection.Roulette(ar, ar.Length - elite.Length);
            var crossed = Crossover.UniformCrossoverPop(selected);
            var mutated = Mutation.MutatePop(crossed, MutationFraction, RandomValueProvider);

            return elite.Concat(mutated);
        }
    }
}