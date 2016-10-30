using Gnn.NeuralNet.Structures.Base;
using Gnn.NeuralNet.Structures.TransferFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.NeuralNet.Structures {

    public class Perceptron : INode {
        public TransferFunction Transfer { get; }

        public INode[] Input { get; protected set; }

        public float Output => CalculateOutput();

        public Perceptron(TransferFunction tf) {
            Transfer = tf;
            Input = new INode[0];
        }

        public float CalculateOutput() {
            var input = Input.Select(i => i.Output);
            var res = Transfer.SumCalculate(input);
            return res;
        }

        public void SetInputs(IEnumerable<INode> newInputs) {
            Input = newInputs.ToArray();
        }

        public void AddInputs(params INode[] addInput) {
            SetInputs(Input.Concat(addInput));
        }

        public void RemoveInputs(params INode[] removeInput) {
            SetInputs(Input.Except(removeInput));
        }
    }
}