using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual.GameObjects.Base {

    public abstract class GameObject : Drawable {
        public Vector2 CenterPosition { get; set; }

        public int Radius { get; }
        public int Diameter { get; }

        public World World { get; }

        public override int DrawX => (int)Math.Round(CenterPosition.X);
        public override int DrawY => (int)Math.Round(CenterPosition.Y);

        public bool Active { get; protected set; } = true;

        public float Lifespan { get; protected set; }

        public bool ShowInfo { get; set; }

        public GameObject(World world, Texture2D texture, Vector2 centerPosition, int? radius = null) : base(texture, DrawableOrigin.Center) {
            World = world;
            Radius = radius ?? (int)Math.Round(texture.Width / 2.0);
            Diameter = Radius * 2;
            CenterPosition = centerPosition;
        }

        public abstract void Move(float secsPassed);

        public virtual void Interact(float secsPassed) {
            Lifespan += secsPassed;
        }
    }
}