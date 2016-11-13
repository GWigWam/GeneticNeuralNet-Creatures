﻿using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Genetic {

    public static class StatHelper {

        public static float[] VarietyPerGene(this IEnumerable<Individual> source) {
            var res = new float[source.First().Values.Length];
            for(int i = 0; i < source.First().Values.Length; i++) {
                res[i] = MathHelper.StdDeviation(source.Select(ind => ind.Values[i]), Grouptype.Population);
            }
            return res;
        }

        public static float AvgVariety(this IEnumerable<Individual> source) {
            return VarietyPerGene(source).Average();
        }
    }
}