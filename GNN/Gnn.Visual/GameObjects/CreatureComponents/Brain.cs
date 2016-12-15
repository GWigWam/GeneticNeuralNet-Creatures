using Gnn.NeuralNet;
using Gnn.NeuralNet.Structures;
using Gnn.NeuralNet.Structures.TransferFunctions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual.GameObjects.CreatureComponents {

    internal class Brain {
        private const int InputCount = Creature.DefEyeCount * 3 + 4;
        private static readonly int[] HiddenCount = new int[] { 20, 6 };
        private const int OutputCount = 3;
        public static TransferFunction Transfer = HyperbolicTangentFunction.Instance;

        public float MinOutput => Transfer.YMin;
        public float MaxOutput => Transfer.YMax;
        public float MinInput => Transfer.XMin;
        public float MaxInput => Transfer.XMax;

        public Network Net { get; private set; }
        public Creature Owner { get; }

        public float Outp_Rotation => Net.Output[0].Output;
        public float Outp_Speed => Net.Output[1].Output;
        public float Outp_Mem => Net.Output[2].Output;

        public Brain(Creature owner, Network initNet = null) {
            Owner = owner;
            Net = initNet ?? CreateDefaultNet();
        }

        public void Update() {
            var memOut = Outp_Mem;

            var indx = 0;

            for(int e = 0; e < Owner.Eyes.Count; e++) {
                var cur = Owner.Eyes.Visible[e];

                var inpR = Transfer.ShiftRange(cur.X, 0, 1);
                var inpG = Transfer.ShiftRange(cur.Y, 0, 1);
                var inpA = Transfer.ShiftRange(cur.Z, 0, 1);
                Net.Input[indx++].Value = inpR;
                Net.Input[indx++].Value = inpG;
                Net.Input[indx++].Value = inpA;
            }

            Net.Input[indx++].Value = Transfer.ShiftRange(Owner.Health, 0, Creature.MaxHealth);
            Net.Input[indx++].Value = memOut;

            Net.Input[indx++].Value = Transfer.ShiftRange(Owner.Speed, 0, Creature.MaxAccelerationPerSecond * 8);
            Net.Input[indx++].Value = Transfer.ShiftRange(Owner.MomentumAngleRelative, -MathHelper.Pi, MathHelper.Pi);
        }

        private static Network CreateDefaultNet() {
            return Network.Create(Transfer, true, InputCount, OutputCount, HiddenCount);
        }
    }
}