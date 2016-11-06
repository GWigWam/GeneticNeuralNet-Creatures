using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Genetic.Procedures {

    public static class Selection {
        private static Random random;

        static Selection() {
            random = new Random();
        }

        /// <summary>
        /// Roulette selction is also known as 'Fitness proportionate selection'
        /// </summary>
        public static IEnumerable<Individual> Roulette(IEnumerable<Individual> source, int outputsize) {
            var totalFitness = source.Sum(i => i.Fitness);

            for(int i = 0; i < outputsize; i++) {
                var cr = random.NextDouble() * totalFitness;

                var passedFitness = 0F;
                var selected = source.First(c => {
                    passedFitness += c.Fitness;
                    return passedFitness > cr;
                });
                yield return selected;
            }
        }
    }
}