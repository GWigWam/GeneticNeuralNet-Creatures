using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual {

    public class MainGameContent {
        public SpriteFont FConsolas { get; private set; }
        public SpriteFont FTrebuchet { get; private set; }

        public Texture2D TTest { get; private set; }
        public Texture2D TCreature { get; private set; }
        public Texture2D TFood { get; private set; }

        public void Init(ContentManager content) {
            FConsolas = content.Load<SpriteFont>("Consolas");
            FTrebuchet = content.Load<SpriteFont>("Trebuchet");

            TTest = content.Load<Texture2D>("test");
            TCreature = content.Load<Texture2D>("Creature");
            TFood = content.Load<Texture2D>("Food");
        }
    }
}