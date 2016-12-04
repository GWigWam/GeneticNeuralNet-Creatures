using Gnn.NeuralNet;
using Gnn.NeuralNet.Structures;
using Gnn.NeuralNet.Structures.TransferFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual.GameObjects.CreatureComponents {

    internal class Brain {
        public TransferFunction Transfer = HyperbolicTangentFunction.Instance;
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
            Net = initNet ?? CreateDefaultNet(Owner.Eyes.Count + 1);
        }

        public void Update() {
            var memOut = Outp_Mem;

            for(int e = 0; e < Owner.Eyes.Count; e++) {
                var cur = Owner.Eyes.Visible[e];

                var inpR = Transfer.ShiftRange(cur.X, 0, 1);
                var inpG = Transfer.ShiftRange(cur.Y, 0, 1);
                var inpA = Transfer.ShiftRange(cur.Z, 0, 1);
                Net.Input[e * 3 + 0].Value = inpR;
                Net.Input[e * 3 + 1].Value = inpG;
                Net.Input[e * 3 + 2].Value = inpA;
            }

            Net.Input[Owner.Eyes.Count + 0].Value = Transfer.ShiftRange(Owner.Health, 0, Creature.MaxHealth);
            Net.Input[Owner.Eyes.Count + 1].Value = memOut;
        }

        private Network CreateDefaultNet(int inpCount) {
            return Network.Create(Transfer, true, inpCount * 3, 3, 15, 5);
        }
    }
}