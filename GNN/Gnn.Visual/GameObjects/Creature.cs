using Gnn.NeuralNet;
using Gnn.NeuralNet.Structures.TransferFunctions;
using Gnn.Visual.GameObjects.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual.GameObjects {

    public class Creature : GameObject {
        public const int VisionDistance = 100;
        public const float MaxRotPerTick = (float)(MathHelper.TwoPi / 50.0);
        public const float DistancePerTick = 5;

        public Network Brain { get; }

        public Creature(Texture2D texture, Vector2 position) : base(texture, position) {
            Brain = Network.Create(HyperbolicTangentFunction.Instance, true, 1, 1, 1);
        }

        public override void Move() {
            var outp = Brain.Output[0].Output;
            var rotChange = Helpers.MathHelper.ShiftRange(outp, Brain.Transfer.YMin, Brain.Transfer.YMax, -MaxRotPerTick, MaxRotPerTick);
            Rotation += rotChange;

            CenterPosition = GeomHelper.GetRelative(CenterPosition, Rotation, DistancePerTick);
        }

        public override void Interact(IEnumerable<GameObject> gameObjs) {
            var notMe = gameObjs.Where(g => g != this);
            SetVisionInputs(notMe);
        }

        public override void Draw(SpriteBatch sb) {
            float r = Helpers.MathHelper.ShiftRange(Brain.Input[0].Value, -2, 2, 0, 1);
            var col = new Color(r, r, r);
            DrawHelper.DrawLine(sb, CenterPosition, GeomHelper.GetRelative(CenterPosition, Rotation, VisionDistance), 1, col);
            base.Draw(sb);
        }

        private void SetVisionInputs(IEnumerable<GameObject> gameObjs) {
            var lineStart = CenterPosition;
            var lineEnd = GeomHelper.GetRelative(CenterPosition, Rotation, VisionDistance);

            GameObject closestObj = null;
            float closestDst = float.MaxValue;
            foreach(var curGameObj in gameObjs) {
                var dist = Vector2.Distance(CenterPosition, curGameObj.CenterPosition) - Radius - curGameObj.Radius;
                if(dist < closestDst) {
                    if(GeomHelper.LineIntersectsCircle(lineStart, lineEnd, curGameObj.CenterPosition, curGameObj.Radius)) {
                        closestObj = curGameObj;
                        closestDst = dist;
                    }
                }
            }

            var inp = Helpers.MathHelper.ShiftRange(VisionDistance - (closestObj == null ? VisionDistance : closestDst), 0, VisionDistance, Brain.Transfer.XMin, Brain.Transfer.XMax);
            Brain.Input[0].Value = inp;
        }
    }
}