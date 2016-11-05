using Gnn.Genetic;
using Gnn.Genetic.Procedures;
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

            var indv = nw.ToIndividual(4);

            var src = Generate(100).ToArray();
            for(int i = 0; i < 100; i++) {
                var fAvg = src.Average(ind => ind.Fitness);
                var fMin = src.Min(ind => ind.Fitness);
                var fMed = src.OrderBy(ind => ind.Fitness).Skip(src.Length / 2).First().Fitness;
                src = new Selection(0.025F).Select(src).OrderByDescending(ind => ind.Fitness).ToArray();
            }

            Console.ReadKey();
        }

        private static IEnumerable<Individual> Generate(int count) {
            for(int i = 0; i < count; i++) {
                yield return new Individual(null, random.Next(0, 100));
            }
        }
    }
}