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

namespace Gnn.Visual.GameObjects.CreatureComponents {

    public class Creature : GameObject, IWorldVisible {
        internal const int DefEyeCount = 5;
        internal const float MaxRotPerSecond = (MathHelper.TwoPi / 2);
        internal const float MaxDistancePerSecond = 600;
        internal const float HealthLostPerSpeedSec = 1 / 15f;
        internal const float IdleHealthLossPerSecond = 1f / 10f;
        internal const float MaxHealth = 4f;

        public Vector2 Color => new Vector2(1, 0);

        private float health = 0.3f;
        public float Health {
            get { return health; }
            set {
                health = value < MaxHealth ? value : MaxHealth;
            }
        }

        internal Vision Eyes;
        internal Brain Brain;

        public Creature(World world, MainGameContent res, Vector2 position, int eyeCount = DefEyeCount, Network initNw = null) : base(world, res.TCreature, position) {
            Eyes = new Vision(this, eyeCount);
            Brain = new Brain(this, initNw);
        }

        public override void Move(float secsPassed) {
            var outp = Brain.Outp_Rotation;
            var rotChange = Helpers.MathHelper.ShiftRange(outp, Brain.MinOutput, Brain.MaxOutput, -MaxRotPerSecond, MaxRotPerSecond);
            Rotation += (rotChange * secsPassed);

            var spd = Helpers.MathHelper.ShiftRange(Brain.Outp_Speed, Brain.MinOutput, Brain.MaxOutput, 0, 1);
            var dist = MaxDistancePerSecond * secsPassed * spd;
            Health -= spd * HealthLostPerSpeedSec * secsPassed;

            CenterPosition = GeomHelper.GetRelative(CenterPosition, Rotation, dist);
        }

        public override void Interact(float secsPassed) {
            base.Interact(secsPassed);
            var notMe = World.ActiveGameObjs.Where(g => g != this);
            Eyes.Update(notMe);

            Brain.Update();

            foreach(var food in World.ActiveGameObjs.OfType<Food>()) {
                var dst = Vector2.Distance(food.CenterPosition, CenterPosition);
                if(dst < Radius + food.Radius) {
                    Health += food.Health;
                    food.Destory();
                }
            }

            Health -= (IdleHealthLossPerSecond * secsPassed);
            if(Health < 0) {
                Active = false;
            }
        }

        public override void Draw(SpriteBatch sb) {
            if(ShowInfo) {
                sb.DrawString(World.Game.Res.FConsolas, $"{Health}", CenterPosition + new Vector2(0, Radius + 3), Microsoft.Xna.Framework.Color.Black);
            }

            Eyes.Draw(sb);
            base.Draw(sb);
        }
    }
}