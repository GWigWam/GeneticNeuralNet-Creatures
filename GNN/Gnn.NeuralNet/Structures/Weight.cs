using Gnn.NeuralNet.Structures.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.NeuralNet.Structures {

    public class Weight : INode {
        internal const float DefValue = 0;

        public float Value { get; set; }
        public INode Alter { get; protected set; }

        public float Output => Alter.Output * Value;

        public Weight(INode alter, float value = DefValue) {
            Alter = alter;
            Value = value;
        }

        public static Weight Weigh(INode alter, float value = DefValue) {
            return new Weight(alter, value);
        }
    }

    public static class WeightExtension {

        public static Weight Weigh(this INode alter, float value = Weight.DefValue) {
            return Weight.Weigh(alter, value);
        }
    }
}