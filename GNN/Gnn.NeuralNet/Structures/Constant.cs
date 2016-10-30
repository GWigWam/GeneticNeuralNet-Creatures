using Gnn.NeuralNet.Structures.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.NeuralNet.Structures {

    public class Constant : INode {
        public float Output { get; }

        public Constant(float value) {
            Output = value;
        }
    }
}