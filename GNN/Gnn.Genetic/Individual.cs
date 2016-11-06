using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Genetic {

    [System.Diagnostics.DebuggerDisplay("{Fitness}")]
    public class Individual {
        public float[] Values { get; }

        public float Fitness { get; }

        public Individual(float[] values, float fitness = -1) {
            Values = values;
            Fitness = fitness;
        }
    }
}