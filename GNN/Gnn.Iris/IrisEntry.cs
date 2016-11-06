using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Iris {

    public enum IrisSpecies {
        Setosa, Versicolor, Virginica
    }

    [System.Diagnostics.DebuggerDisplay("{Species}: [sl:{SepalLength}, sw:{SepalWidth}, pl:{PetalLength}, pw:{PetalWidth}]")]
    public struct IrisEntry {
        public float SepalLength { get; }
        public float SepalWidth { get; }
        public float PetalLength { get; }
        public float PetalWidth { get; }
        public IrisSpecies Species { get; }

        public IrisEntry(float sl, float sw, float pl, float pw, IrisSpecies species) {
            SepalLength = sl;
            SepalWidth = sw;
            PetalLength = pl;
            PetalWidth = pw;
            Species = species;
        }
    }
}