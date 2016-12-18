using Gnn.Visual.GameObjects.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual.GameObjects {

    public class Food : GameObject, IWorldVisible {
        internal const float MaxHealth = 0.5f;
        internal const float AddHealthPerSecond = MaxHealth / 12f;

        private float health;

        public float Health {
            get { return health; }
            set { health = value > MaxHealth ? MaxHealth : value; }
        }

        public Vector2 Color => new Vector2(0, health / MaxHealth);

        public Food(World world, MainGameContent res, Vector2 position) : base(world, res.TFood, position) {
        }

        public override void Interact(float secsPassed) {
            base.Interact(secsPassed);
            Health += AddHealthPerSecond * secsPassed;
        }

        public override void Move(float secsPassed) {
            if(!World.HighPerformanceMode) {
                Rotation += (0.01f * (float)Math.Pow((health / MaxHealth) * 100, 1.3)) * secsPassed;
            }
        }

        public override void Draw(SpriteBatch sb) {
            if(ShowInfo) {
                sb.DrawString(World.Game.Res.FConsolas, $"{Health}", CenterPosition + new Vector2(0, Radius + 3), Microsoft.Xna.Framework.Color.Black);
            }

            var rg = Color;
            var col = new Color(rg.X, rg.Y, 0, (Health / MaxHealth) + 0.3f);
            sb.Draw(Texture, new Rectangle(DrawX, DrawY, Texture.Width, Texture.Height), null, col, Rotation, Origin, SpriteEffects.None, 0);
        }

        public void Destory() => Active = false;
    }
}