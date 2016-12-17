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

        internal const float MaxRotPerSecond = MathHelper.Pi / 2;

        internal const float MaxAccelerationPerSecond = 20f;
        internal const float AccelerationLossPerSecond = 20f;
        internal const float MaxSpeedPerSec = 600;

        internal const float HealthLostOnMaxAcceleration = 1 / 20f;
        internal const float IdleHealthLossPerSecond = 1f / 10f;
        internal const float MaxHealth = 5f;

        public Vector2 Color => new Vector2(1, 0);

        private float health = 1f;
        public float Health {
            get { return health; }
            set {
                health = value < MaxHealth ? value : MaxHealth;
            }
        }

        public Vector2 Momentum { get; set; }
        public float Speed => Momentum.Length();
        public float MomentumAngle => GeomHelper.Angle(0, 0, Momentum.X, Momentum.Y);
        public float MomentumAngleRelative => GeomHelper.RadRange_Pi(Rotation - MomentumAngle);

        internal Vision Eyes;
        internal Brain Brain;

        public Creature(World world, MainGameContent res, Vector2 position, int eyeCount = DefEyeCount, Network initNw = null) : base(world, res.TCreature, position) {
            Eyes = new Vision(this, eyeCount);
            Brain = new Brain(this, initNw);
        }

        public override void Move(float secsPassed) {
            var rotationDelta = Helpers.MathHelper.ShiftRange(Brain.Outp_Rotation, Brain.MinOutput, Brain.MaxOutput, -MaxRotPerSecond, MaxRotPerSecond);
            Rotation += (rotationDelta * secsPassed);
            var momLostX = Momentum.X > 0 ?
                Math.Max(0, Momentum.X - AccelerationLossPerSecond * (float)Math.Abs(Math.Cos(Rotation)) * secsPassed) :
                Math.Min(0, Momentum.X + AccelerationLossPerSecond * (float)Math.Abs(Math.Cos(Rotation)) * secsPassed);
            var momLostY = Momentum.Y > 0 ?
                Math.Max(0, Momentum.Y - AccelerationLossPerSecond * (float)Math.Abs(Math.Sin(Rotation)) * secsPassed) :
                Math.Min(0, Momentum.Y + AccelerationLossPerSecond * (float)Math.Abs(Math.Sin(Rotation)) * secsPassed);
            Momentum = new Vector2(momLostX, momLostY);

            if(Speed < MaxSpeedPerSec * secsPassed) {
                var accelerationFraction = Helpers.MathHelper.ShiftRange(Brain.Outp_Speed, Brain.MinOutput, Brain.MaxOutput, 0, 1);
                var acceleration = accelerationFraction * (MaxAccelerationPerSecond + AccelerationLossPerSecond) * secsPassed;
                var accelerationDelta = GeomHelper.CreateVector(Rotation, acceleration);
                Momentum += accelerationDelta;

                Health -= accelerationFraction * HealthLostOnMaxAcceleration * secsPassed;
            }
            CenterPosition += Momentum;
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
            Eyes.Draw(sb);
            base.Draw(sb);

            if(ShowInfo) {
                const int spacing = 6;
                var netStr = string.Empty;
                bool bottomReached = false;
                for(int depth = 0; !bottomReached; depth++) {
                    netStr += depth < Brain.Net.Input.Length ? $"{Brain.Net.Input[depth].Output,-spacing:n2} ->" : "         ";

                    for(int hid = 0; hid < Brain.Net.Hidden.Length; hid++) {
                        netStr += depth < Brain.Net.Hidden[hid].Length ? $" |{Brain.Net.Hidden[hid][depth].Output,-spacing:n2}" : "        ";
                    }

                    netStr += depth < Brain.Net.Output.Length ? $" -> {Brain.Net.Output[depth].Output,-spacing:n2}" : "          ";

                    netStr += "\n";
                    bottomReached = depth >= Math.Max(Brain.Net.Input.Length, Brain.Net.Output.Length) && depth >= Brain.Net.Hidden.Max(h => h.Length);
                }

                sb.DrawString(World.Game.Res.FConsolas, $"H={Health}\n{netStr}", CenterPosition + new Vector2(0, Radius + 3), Microsoft.Xna.Framework.Color.Black);
                DrawHelper.DrawLine(sb, CenterPosition, GeomHelper.GetRelative(CenterPosition, MomentumAngle, Speed * 30), 1, Microsoft.Xna.Framework.Color.DarkBlue);
            }
        }
    }
}