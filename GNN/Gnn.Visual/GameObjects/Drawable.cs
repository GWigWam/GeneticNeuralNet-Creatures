﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual.GameObjects {

    public abstract class Drawable {
        public virtual Texture2D Texture { get; set; }

        private float rotation = 0;

        public float Rotation {
            get { return rotation; }
            set {
                rotation = value % MathHelper.TwoPi;
            }
        }

        private Vector2 Origin { get; }

        public abstract int DrawX { get; }
        public abstract int DrawY { get; }

        public Drawable(Texture2D texture, DrawableOrigin origin) {
            Texture = texture;
            Origin = origin == DrawableOrigin.Center ? new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f) : Vector2.Zero;
        }

        public virtual void Draw(SpriteBatch sb) {
            sb.Draw(Texture, new Rectangle(DrawX, DrawY, Texture.Width, Texture.Height), null, Color.White, Rotation, Origin, SpriteEffects.None, 0);
        }
    }

    public enum DrawableOrigin {
        TopLeft, Center
    }
}