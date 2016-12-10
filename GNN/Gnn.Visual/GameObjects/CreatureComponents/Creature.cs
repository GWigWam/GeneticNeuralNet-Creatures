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

        internal const float MaxRotPerSecond = (MathHelper.Pi / 2);

        internal const float MaxAccelerationPerSecond = 2.5f;
        internal const float AccelerationLossPerSecond = 0.8f;

        internal const float HealthLostPerAcceleration = 1 / 15f;
        internal const float IdleHealthLossPerSecond = 1f / 10f;
        internal const float MaxHealth = 4f;

        public Vector2 Color => new Vector2(1, 0);

        private float health = 1f;
        public float Health {
            get { return health; }
            set {
                health = value < MaxHealth ? value : MaxHealth;
            }
        }

        public Vector2 Momentum { get; set; }

        internal Vision Eyes;
        internal Brain Brain;

        public Creature(World world, MainGameContent res, Vector2 position, int eyeCount = DefEyeCount, Network initNw = null) : base(world, res.TCreature, position) {
            Eyes = new Vision(this, eyeCount);
            Brain = new Brain(this, initNw);
        }

        public override void Move(float secsPassed) {
            var rotationDelta = Helpers.MathHelper.ShiftRange(Brain.Outp_Rotation, Brain.MinOutput, Brain.MaxOutput, -MaxRotPerSecond, MaxRotPerSecond);
            Rotation += (rotationDelta * secsPassed);

            var momLossX = Momentum.X > 0 ?
                Math.Max(0, Momentum.X - AccelerationLossPerSecond * (float)Math.Cos(Rotation) * secsPassed) :
                Math.Min(0, Momentum.X + AccelerationLossPerSecond * (float)Math.Cos(Rotation) * secsPassed);
            var momLossY = Momentum.Y > 0 ?
                Math.Max(0, Momentum.Y - AccelerationLossPerSecond * (float)Math.Sin(Rotation) * secsPassed) :
                Math.Min(0, Momentum.Y + AccelerationLossPerSecond * (float)Math.Sin(Rotation) * secsPassed);
            Momentum = new Vector2(momLossX, momLossY);

            var accelerationFraction = Helpers.MathHelper.ShiftRange(Brain.Outp_Speed, Brain.MinOutput, Brain.MaxOutput, 0, 1);
            var acceleration = accelerationFraction * MaxAccelerationPerSecond * secsPassed;
            var accelerationDelta = GeomHelper.CreateVector(Rotation, acceleration);
            Momentum += accelerationDelta;
            CenterPosition += Momentum;

            Health -= accelerationFraction * HealthLostPerAcceleration * secsPassed;
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