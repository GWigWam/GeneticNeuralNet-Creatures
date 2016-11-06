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

        //private static TransferFunction Transfer = new HyperbolicTangentFunction();
        private static TransferFunction Transfer = new SigmoidFunction();

        private static void Main(string[] args) {
            var data = DataReader.ReadFromFile("data/iris.data").OrderByDescending(i => i.SepalWidth).ToArray();
            data = NormalizeData(data, Transfer.XMin, Transfer.XMax).ToArray();
            var networks = GenerateNetworks().ToArray();
            var genetic = new Genetic.Genetic(0.02F, 0.001F, NextFloat);
            var indivs = new Individual[NetworkCount];

            int genIndx = 0;
            float avgFitness = float.MaxValue;
            var startT = Environment.TickCount;
            do {
                var t = Environment.TickCount;
                Console.Write($"\nGen: {genIndx}");
                Parallel.For(0, NetworkCount, (n) => {
                    var curNet = networks[n];
                    var grade = Grade(curNet, data);
                    indivs[n] = curNet.ToIndividual(grade);
                });
                var res = genetic.Apply(indivs).ToArray();
                for(int n = 0; n < NetworkCount; n++) {
                    networks[n].SetWeights(res[n]);
                }
                avgFitness = indivs.Average(ind => ind.Fitness);
                Console.Write($" - Avg fitness: {avgFitness:N3}, variety: {indivs.AvgVariety():N3}, time {Environment.TickCount - t}Ms");
                genIndx++;
            } while(avgFitness < 0.95);

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

        private static float NextFloat() => MathHelper.Random(Transfer.XMin, Transfer.XMax);

        private static IEnumerable<IrisEntry> NormalizeData(IrisEntry[] data, float min, float max) {
            foreach(var entry in data) {
                //TODO: it is suboptimal to calculate min and max every time
                var petalLength = MathHelper.ShiftRange(entry.PetalLength, data.Min(i => i.PetalLength), data.Max(i => i.PetalLength), min, max);
                var petalWidth = MathHelper.ShiftRange(entry.PetalWidth, data.Min(i => i.PetalWidth), data.Max(i => i.PetalWidth), min, max);
                var sepalLength = MathHelper.ShiftRange(entry.SepalLength, data.Min(i => i.SepalLength), data.Max(i => i.SepalLength), min, max);
                var sepalWidth = MathHelper.ShiftRange(entry.SepalWidth, data.Min(i => i.SepalWidth), data.Max(i => i.SepalWidth), min, max);

                yield return new IrisEntry(sepalLength, sepalWidth, petalLength, petalWidth, entry.Species);
            }
        }
    }
}