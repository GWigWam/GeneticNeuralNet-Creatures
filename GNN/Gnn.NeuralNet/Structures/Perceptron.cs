using Gnn.NeuralNet.Structures.Base;
using Gnn.NeuralNet.Structures.TransferFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.NeuralNet.Structures {

    public class Perceptron : INode {

        public event Action OutputChanged;

        public TransferFunction Transfer { get; }

        public INode[] Input { get; protected set; }

        public float Output {
            get {
                if(!CacheValid) {
                    UpdateCache();
                }
                return CacheValue;
            }
        }

        private bool CacheValid = false;
        private float CacheValue = -1;

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
            foreach(var oldInp in Input) {
                oldInp.OutputChanged -= Inp_OutputChanged;
            }

            Input = newInputs.ToArray();

            foreach(var inp in Input) {
                inp.OutputChanged += Inp_OutputChanged;
            }
        }

        public void AddInputs(params INode[] addInput) {
            SetInputs(Input.Concat(addInput));
        }

        public void RemoveInputs(params INode[] removeInput) {
            SetInputs(Input.Except(removeInput));
        }

        private void Inp_OutputChanged() {
            CacheValid = false;
            OutputChanged?.Invoke();
        }

        private void UpdateCache() {
            CacheValue = CalculateOutput();
            CacheValid = true;
        }
    }
}