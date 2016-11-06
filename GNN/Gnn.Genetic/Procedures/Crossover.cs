using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Genetic.Procedures {

    public static class Crossover {

        public static IEnumerable<Individual> UniformCrossoverPop(IEnumerable<Individual> source) {
            var shuffled = source.OrderBy(i => MathHelper.Random()).ToArray();
            for(int i = 1; i < shuffled.Length; i += 2) {
                var cRes = UniformCrossover(shuffled[i - 1], shuffled[i]);
                yield return cRes.Item1;
                yield return cRes.Item2;
            }
        }

        public static Tuple<Individual, Individual> UniformCrossover(Individual in0, Individual in1) {
            var length = in0.Values.Length;
            var r0 = new float[length];
            var r1 = new float[length];

            for(int i = 0; i < length; i++) {
                var cross = MathHelper.Random() < 0.5;
                r0[i] = cross ? in1.Values[i] : in0.Values[i];
                r1[i] = cross ? in0.Values[i] : in1.Values[i];
            }

            return Tuple.Create(new Individual(r0), new Individual(r1));
        }
    }
}