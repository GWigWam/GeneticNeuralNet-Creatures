using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual.GameObjects {

    public abstract class GameObject : Drawable {
        public Vector2 CenterPosition { get; set; }

        public int Radius { get; }
        public int Diameter { get; }

        public override int DrawX => (int)Math.Round(CenterPosition.X);
        public override int DrawY => (int)Math.Round(CenterPosition.Y);

        public GameObject(Texture2D texture, Vector2 centerPosition, int? radius = null) : base(texture, DrawableOrigin.Center) {
            Radius = radius ?? (int)Math.Round(texture.Width / 2.0);
            Diameter = Radius * 2;
            CenterPosition = centerPosition;
        }

        public abstract void Move();

        public abstract void Interact(GameObject[] gameObjs);
    }
}