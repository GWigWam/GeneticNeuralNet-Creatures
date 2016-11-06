using Gnn.Genetic;
using Gnn.Genetic.Procedures;
using Gnn.NeuralNet;
using Gnn.NeuralNet.Structures.TransferFunctions;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Iris {

    public class Program {
        private const int NetworkCount = 500;
        private static Random random = new Random();

        //private static TransferFunction Transfer = new HyperbolicTangentFunction();
        private static TransferFunction Transfer = new SigmoidFunction();

        private static void Main(string[] args) {
            var data = DataReader.ReadFromFile("data/iris.data").OrderByDescending(i => i.SepalWidth).ToArray();
            data = NormalizeData(data, Transfer.XMin, Transfer.XMax).ToArray();
            var networks = GenerateNetworks().ToArray();
            var genetic = new Genetic.Genetic(0.02F, 0.01F, NextFloat);
            var indivs = new Individual[NetworkCount];

            var startT = Environment.TickCount;
            for(int g = 0; g < 100; g++) {
                var t = Environment.TickCount;
                Console.Write($"\nGen: {g}");
                for(int n = 0; n < NetworkCount; n++) {
                    var curNet = networks[n];
                    var grade = Grade(curNet, data);
                    indivs[n] = curNet.ToIndividual(grade);
                }
                var res = genetic.Apply(indivs).ToArray();
                for(int n = 0; n < NetworkCount; n++) {
                    networks[n].SetWeights(res[n]);
                }
                Console.Write($" - Avg fitness: {indivs.Average(ind => ind.Fitness):N3}, time {Environment.TickCount - t}Ms");
            }

            Console.Write($"\n\nDone in {Environment.TickCount - startT}Ms");
            Console.ReadKey();
        }

        private static IEnumerable<Network> GenerateNetworks() {
            for(int n = 0; n < NetworkCount; n++) {
                yield return Network.Create(Transfer, true, 4, 3);
            }
        }

        private static float Grade(Network nw, IrisEntry[] data) {
            var successCount = 0;
            foreach(var d in data) {
                var res = nw.GetOutput(d.PetalLength, d.PetalWidth, d.SepalLength, d.SepalWidth);

                var index = IndexOfMax(res);
                var success = d.Species == (IrisSpecies)index;
                successCount += success ? 1 : 0;
            }

            return successCount / (float)data.Length;
        }

        private static int IndexOfMax(float[] ar) {
            float max = ar.Max();
            for(int i = 0; i < ar.Length; i++) {
                if(ar[i] == max) {
                    return i;
                }
            }
            throw new Exception("What...");
        }

        private static float NextFloat() {
            return (float)(random.NextDouble() * 2) - 1;
        }

        private static IEnumerable<IrisEntry> NormalizeData(IrisEntry[] data, float min, float max) {
            foreach(var entry in data) {
                //TODO: it is suboptimal to calculate min and max every time
                var petalLength = (float)MathHelper.ShiftRange(entry.PetalLength, data.Min(i => i.PetalLength), data.Max(i => i.PetalLength), min, max);
                var petalWidth = (float)MathHelper.ShiftRange(entry.PetalWidth, data.Min(i => i.PetalWidth), data.Max(i => i.PetalWidth), min, max);
                var sepalLength = (float)MathHelper.ShiftRange(entry.SepalLength, data.Min(i => i.SepalLength), data.Max(i => i.SepalLength), min, max);
                var sepalWidth = (float)MathHelper.ShiftRange(entry.SepalWidth, data.Min(i => i.SepalWidth), data.Max(i => i.SepalWidth), min, max);

                yield return new IrisEntry(sepalLength, sepalWidth, petalLength, petalWidth, entry.Species);
            }
        }
    }
}