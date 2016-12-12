using Gnn.NeuralNet.Structures.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.NeuralNet.Structures {

    [DebuggerDisplay("V: [{Output}]")]
    public class Variable : INode {

        public event Action OutputChanged;

        private float val;

        public float Value {
            get { return val; }
            set {
                if(val != value) {
                    val = value;
                    OutputChanged?.Invoke();
                }
            }
        }

        public float Output => Value;

        public Variable(float init = 0) {
            Value = init;
        }
    }
}