using Gnn.Visual.GameObjects.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual.GameObjects.CreatureComponents {

    internal class Propellant : Base.IDrawable {
        private Creature Owner;
        private MainGameContent Res;

        internal const float ParticleMaxAgeSec = 0.15f;
        internal const float SpawnParticlesPerAcceleration = 1f / 10;

        public List<PropellantParticle> Particles { get; }

        public Propellant(Creature owner, MainGameContent r) {
            Particles = new List<PropellantParticle>();
            Owner = owner;
            Res = r;
        }

        public void Move(float secsPassed, Vector2 accelerationDelta) {
            if(!Owner.World.HighPerformanceMode) {
                if(ThreadSafeRandom.Instance.NextDouble() <= accelerationDelta.Length() * SpawnParticlesPerAcceleration) {
                    var momentum = accelerationDelta / 3;
                    momentum.X += momentum.X * (float)(ThreadSafeRandom.Instance.NextDouble() - 0.5);
                    momentum.Y += momentum.Y * (float)(ThreadSafeRandom.Instance.NextDouble() - 0.5);
                    momentum = (momentum / secsPassed) * -1;
                    var @new = new PropellantParticle(Res.TPropellantParticle, Owner.CenterPosition, momentum);
                    Particles.Add(@new);
                }

                foreach(var part in Particles) {
                    part.Move(secsPassed);
                }
            }

            Particles.RemoveAll(p => p.Age > ParticleMaxAgeSec);
        }

        public void Draw(SpriteBatch sb) {
            foreach(var part in Particles) {
                part.Draw(sb);
            }
        }
    }

    internal class PropellantParticle : Drawable, IMoves {
        public Vector2 Position { get; set; }
        public Vector2 Momentum { get; set; }
        public float Age { get; private set; }

        public override int DrawX => (int)Math.Round(Position.X);
        public override int DrawY => (int)Math.Round(Position.Y);

        public PropellantParticle(Texture2D texture, Vector2 pos, Vector2 momentum) : base(texture, DrawableOrigin.Center) {
            Position = pos;
            Momentum = momentum;
        }

        public void Move(float secsPassed) {
            Age += secsPassed;
            Position += Momentum * secsPassed;
            Rotation += MathHelper.PiOver2;
        }
    }
}