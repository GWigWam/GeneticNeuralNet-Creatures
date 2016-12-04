using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Genetic.Procedures {

    public static class Selection {

        /// <summary>
        /// Roulette selction is also known as 'Fitness proportionate selection'
        /// </summary>
        public static IEnumerable<Individual> Roulette(IEnumerable<Individual> source, int outputsize) {
            var totalFitness = source.Sum(i => i.Fitness);

            for(int i = 0; i < outputsize; i++) {
                var cr = MathHelper.Random() * totalFitness;

                var passedFitness = 0F;
                var selected = source.FirstOrDefault(c => {
                    passedFitness += c.Fitness;
                    return passedFitness > cr;
                }) ?? source.Last();
                yield return selected;
            }
        }
    }
}