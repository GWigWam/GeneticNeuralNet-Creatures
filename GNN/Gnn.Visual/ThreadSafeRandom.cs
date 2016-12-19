using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual
{
    public static class ThreadSafeRandom
    {
        private static readonly Random SeedGenerator = new Random(4);

        [ThreadStatic]
        private static Random ThreadRandom;

        public static Random Instance {
            get {
                if (ThreadRandom == null) {
                    int seed;
                    lock (SeedGenerator) {
                        seed = SeedGenerator.Next();
                    }
                    ThreadRandom = new Random(seed);
                }
                return ThreadRandom;
            }
        }
    }
}
