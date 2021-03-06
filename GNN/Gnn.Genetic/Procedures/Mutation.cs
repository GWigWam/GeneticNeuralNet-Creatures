﻿using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Genetic.Procedures {

    public static class Mutation {

        public static IEnumerable<Individual> MutatePop(IEnumerable<Individual> source, float mutationFraction, Func<float> randomValueProvider) {
            foreach(var inp in source) {
                yield return Mutate(inp, mutationFraction, randomValueProvider);
            }
        }

        public static Individual Mutate(Individual input, float mutationFraction, Func<float> randomValueProvider) {
            for(int i = 0; i < input.Values.Length; i++) {
                if(MathHelper.Random() < mutationFraction) {
                    input.Values[i] = randomValueProvider();
                }
            }
            return input;
        }
    }
}