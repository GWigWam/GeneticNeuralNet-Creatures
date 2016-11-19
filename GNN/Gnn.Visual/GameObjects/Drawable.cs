using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual.GameObjects {

    public abstract class Drawable {
        public virtual Texture2D Texture { get; set; }

        public virtual float Rotation { get; set; }

        public abstract int TopLeftX { get; }
        public abstract int TopLeftY { get; }

        public Drawable(Texture2D texture) {
            Texture = texture;
        }

        public virtual void Draw(SpriteBatch sb) {
            sb.Draw(Texture, new Rectangle(TopLeftX, TopLeftY, Texture.Width, Texture.Height), null, Color.White, Rotation, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}