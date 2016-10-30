using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Iris {

    public enum IrisSpecies {
        Setosa, Versicolor, Virginica
    }

    public struct IrisEntry {
        public double SepalLength { get; }
        public double SepalWidth { get; }
        public double PetalLength { get; }
        public double PetalWidth { get; }
        public IrisSpecies Species { get; }

        public IrisEntry(double sl, double sw, double pl, double pw, IrisSpecies species) {
            SepalLength = sl;
            SepalWidth = sw;
            PetalLength = pl;
            PetalWidth = pw;
            Species = species;
        }
    }
}