using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Genetic.Procedures {

    public static class Elitism {

        public static IEnumerable<Individual> SelectElite(IEnumerable<Individual> source, float elitismFraction) {
            var take = (int)Math.Round(source.Count() * elitismFraction);
            return source.OrderByDescending(i => i.Fitness).Take(take);
        }
    }
}