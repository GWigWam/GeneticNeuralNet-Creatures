using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Genetic.Procedures {

    public class Selection {
        public float EletismFraction { get; }

        private Random random;

        public Selection(float eletismFraction) {
            EletismFraction = eletismFraction;
            random = new Random();
        }

        public IEnumerable<Individual> Select(Individual[] source, int? outputsize = null) {
            var desiredSize = outputsize ?? source.Length;

            var elite = Eletism(source);
            var selected = Roulette(source, desiredSize - elite.Count());

            return elite.Concat(selected);
        }

        private IEnumerable<Individual> Roulette(Individual[] source, int outputsize) {
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

        private IEnumerable<Individual> Eletism(Individual[] source) {
            var take = (int)Math.Round(source.Length * EletismFraction);
            return source.OrderByDescending(i => i.Fitness).Take(take);
        }
    }
}