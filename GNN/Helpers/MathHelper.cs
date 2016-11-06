using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {

    public static class MathHelper {

        // chosen by fair dice roll
        // guaranteed to be random
        private const int Seed = 4;

        private static Random Rand = new Random(Seed);

        public static float Normalize(float input, float min, float max) {
            var val = (input - min) / (max - min);
            return val;
        }

        public static IEnumerable<float> Normalize(IEnumerable<float> collection) {
            float min = collection.Min();
            float max = collection.Max();

            return collection.Select(f => Normalize(f, min, max));
        }

        public static float ShiftRange(float value, float orgMin, float orgMax, float newMin, float newMax) {
            var val = (((value - orgMin) / (orgMax - orgMin)) * (newMax - newMin)) + newMin;
            return val;
        }

        public static float GaussianRandom(float stdDev, float mean) {
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(Rand.NextDouble())) * Math.Sin(2.0 * Math.PI * Rand.NextDouble()); //random normal(0,1)
            float randNormal = (float)(mean + stdDev * randStdNormal); //random normal(mean,stdDev^2)

            return randNormal;
        }

        public static float StdDeviation(IEnumerable<float> values, Grouptype type) {
            var stdDev = Math.Sqrt(Variance(values, type));
            return (float)stdDev;
        }

        public static float Variance(IEnumerable<float> values, Grouptype type) {
            var avg = values.Average();
            var sse = values.Sum(f => Math.Pow(f - avg, 2));

            var avgSse = sse / (values.Count() - (type == Grouptype.Population ? 0 : 1));
            return (float)avgSse;
        }

        public static float Random(float min = 0, float max = 1) {
            return (float)(Rand.NextDouble() * (max - min) + min);
        }
    }

    public enum Grouptype {
        Population, Sample
    }
}