using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual {

    public class MainGameContent {
        public SpriteFont Consolas { get; private set; }

        public void Init(ContentManager content) {
            Consolas = content.Load<SpriteFont>("Consolas");
        }
    }
}