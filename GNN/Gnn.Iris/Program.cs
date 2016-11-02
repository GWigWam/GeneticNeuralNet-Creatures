using Gnn.NeuralNet;
using Gnn.NeuralNet.Structures.TransferFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Iris {

    public class Program {
        private const int TrainSetSize = 130;
        private static Random random = new Random();

        private static void Main(string[] args) {
            var data = DataReader.ReadFromFile("data/iris.data")/*.OrderBy(i => random.Next())*/;

            var nw = Network.Create(new HyperbolicTangentFunction(), true, 4, 3, 4, 6);

            foreach(var d in data) {
                var r = nw.GetOutput(d.PetalLength, d.PetalWidth, d.SepalLength, d.SepalWidth);
            }

            Console.ReadKey();
        }
    }
}