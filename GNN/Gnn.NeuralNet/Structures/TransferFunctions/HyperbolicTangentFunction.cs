using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.NeuralNet.Structures.TransferFunctions {

    public class HyperbolicTangentFunction : TransferFunction {
        public override float XMax => 2;
        public override float XMin => -2;
        public override float YMax => 1;
        public override float YMin => -1;

        public override float Calculate(float input) {
            var val = Math.Tanh(input);
            return (float)val;
        }
    }
}