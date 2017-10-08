using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.NeuralNet.Structures.TransferFunctions {

    /// <summary>
    /// Rectified Linear Unit function
    /// </summary>
    public class ReLUFunction : TransferFunction {

        public static ReLUFunction Instance { get; } = new ReLUFunction();

        public override float YMax => 1;

        public override float YMin => 0;

        public override float XMax => 1;

        public override float XMin => -1;

        public override float Calculate(float input) => input < 0 ? 0 : input;
    }
}
