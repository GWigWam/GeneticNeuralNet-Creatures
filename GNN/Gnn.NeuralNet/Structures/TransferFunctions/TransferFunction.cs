using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.NeuralNet.Structures.TransferFunctions {

    public abstract class TransferFunction {
        public abstract float YMax { get; }
        public abstract float YMin { get; }
        public abstract float XMax { get; }
        public abstract float XMin { get; }

        public float SumCalculate(params float[] input) => SumCalculate((IEnumerable<float>)input);

        public float SumCalculate(IEnumerable<float> input) => Calculate(input.Sum());

        public abstract float Calculate(float input);
    }
}