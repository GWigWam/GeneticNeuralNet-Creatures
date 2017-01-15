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

        private List<GeneticStats> history;

        public IEnumerable<GeneticStats> History => history;

        public Genetic(GeneticParamsProvider paramsProvider, Func<float> randomValueProvider) {
            ParamsProvider = paramsProvider;
            RandomValueProvider = randomValueProvider;
            history = new List<GeneticStats>();
        }

        public IEnumerable<Individual> Apply(IEnumerable<Individual> source) {
            var prms = ParamsProvider(this);
            var inp = source.ToArray();

            var elite = Elitism.SelectElite(inp, prms.ElitismFraction).ToArray();
            if(elite.Length % 2 != 0) {
                throw new InvalidOperationException("Elitism fraction must result in an even number of individuals");
            }
            var selected = Selection.Roulette(inp, inp.Length - elite.Length);
            var crossed = Crossover.UniformCrossoverPop(selected);
            var mutated = Mutation.MutatePop(crossed, prms.MutationFraction, RandomValueProvider);

            var retPop = elite.Concat(mutated).ToArray();
            history.Add(CalcStats(prms, inp, retPop));
            return retPop;
        }

        private GeneticStats CalcStats(GeneticParams prms, Individual[] before, Individual[] after) {
            float min = float.MaxValue;
            float max = float.MinValue;
            float sum = 0;
            foreach(var bef in before) {
                var f = bef.Fitness;
                min = f < min ? f : min;
                max = f > max ? f : max;
                sum += f;
            }
            float avg = sum / before.Length;

            var variety = after.AvgVariety();

            var stats = new GeneticStats {
                GenCount = history.Count + 1,
                MinFitness = min,
                MaxFitness = max,
                AvgFitness = avg,
                Variety = variety,
                UsedParams = prms
            };

            return stats;
        }
    }

    public class GeneticStats {
        public int GenCount { get; set; }

        public float AvgFitness { get; set; }
        public float MaxFitness { get; set; }
        public float MinFitness { get; set; }
        public float Variety { get; set; }

        public GeneticParams UsedParams { get; set; }
    }
}