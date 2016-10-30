using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.NeuralNet.Structures.TransferFunctions {

    public class SigmoidFunction : TransferFunction {
        public override float XMax => 4;
        public override float XMin => -4;
        public override float YMax => 1;
        public override float YMin => 0;

        public override float Calculate(float input) {
            var val = (1.0 / (1.0 + Math.Pow(Math.E, -input)));
            return (float)val;
        }
    }
}