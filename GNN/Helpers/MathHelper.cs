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

        public static float StdDeviation(float[] values) {
            return (float)Math.Sqrt(Variance(values));
        }

        public static float Variance(float[] values) {
            var avg = values.Average();
            return (float)values.Average(f => Math.Pow(f - avg, 2));
        }
    }
}