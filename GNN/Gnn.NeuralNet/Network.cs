using Gnn.NeuralNet.Structures;
using Gnn.NeuralNet.Structures.Base;
using Gnn.NeuralNet.Structures.TransferFunctions;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.NeuralNet {

    public class Network {
        public Variable[] Input { get; protected set; }
        public INode[][] Hidden { get; protected set; }
        public INode[] Output { get; protected set; }
        public Constant Bias { get; protected set; }

        public TransferFunction Transfer { get; protected set; }

        private Network(TransferFunction transfer, bool hasBias) {
            if(transfer == null) {
                throw new ArgumentNullException(nameof(transfer));
            }
            Transfer = transfer;

            Bias = hasBias ? new Constant(1) : null;
        }

        public static Network Create(TransferFunction transfer, bool hasBias, int inputCount, int outputCount, params int[] hiddenCount) {
            var nw = new Network(transfer, hasBias);
            nw.Bias = hasBias ? new Constant(1) : null;
            nw.Hidden = new INode[hiddenCount.Length][];

            nw.Input = CreateVariables(inputCount).ToArray();

            INode[] prevLayer = nw.Input;
            for(int layerIndex = 0; layerIndex <= hiddenCount.Length; layerIndex++) {
                var isOutputLayer = layerIndex == hiddenCount.Length;
                var curLayerHeight = isOutputLayer ? outputCount : hiddenCount[layerIndex];
                var curLayer = CreateNodes(curLayerHeight, nw.Transfer).ToArray();

                foreach(var cur in curLayer) {
                    cur.AddInputs(prevLayer.Select(prev => prev.Weigh(RandomWeight(transfer))).ToArray());
                    if(hasBias) {
                        cur.AddInputs(nw.Bias.Weigh(RandomWeight(transfer)));
                    }
                }

                if(isOutputLayer) {
                    nw.Output = curLayer;
                } else {
                    nw.Hidden[layerIndex] = curLayer;
                    prevLayer = curLayer;
                }
            }

            return nw;
        }

        public float[] GetOutput(params float[] input) {
            for(int i = 0; i < Input.Length; i++) {
                Input[i].Value = input[i];
            }
            return Output.Select(o => o.Output).ToArray();
        }

        private static IEnumerable<Variable> CreateVariables(int count) {
            for(int i = 0; i < count; i++) {
                yield return new Variable();
            }
        }

        private static IEnumerable<Perceptron> CreateNodes(int count, TransferFunction transfer) {
            for(int i = 0; i < count; i++) {
                yield return new Perceptron(transfer);
            }
        }

        private static float RandomWeight(TransferFunction transfer) {
            var xRange = Math.Abs(transfer.XMax - transfer.XMin);
            // 95% falls whitin 2 standard deviations (instead of  68% for 1)
            // div by 2 because this covers both positive and negative range
            // div by 2 again to convert 1-stdev to 2-stdev
            var stdDev = xRange / 2 / 2;

            var res = MathHelper.GaussianRandom(stdDev, 0);
            return (float)res;
        }
    }
}