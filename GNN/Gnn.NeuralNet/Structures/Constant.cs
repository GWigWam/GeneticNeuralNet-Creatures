using Gnn.NeuralNet.Structures.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.NeuralNet.Structures {

    [DebuggerDisplay("C: [{Output}]")]
    public class Constant : INode {

        public event Action OutputChanged;

        public float Output { get; }

        public Constant(float value) {
            Output = value;
            OutputChanged?.Invoke();
        }
    }
}