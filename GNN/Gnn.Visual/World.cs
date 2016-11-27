using Gnn.Visual.GameObjects;
using Gnn.Visual.GameObjects.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual {

    public class World {
        public const int CreatureCount = 200;
        public const int FoodCount = 200;
        public const int WorldSize = 2000;

        public List<GameObject> GameObjs { get; }

        private MainGame Game { get; }

        public World(MainGame game) {
            Game = game;
            GameObjs = new List<GameObject>();
        }

        public void Initialize() {
            Populate();
        }

        public void Update(MouseState mState, KeyboardState kState, Point mPosInWorld, float secsPassed) {
            Parallel.ForEach(GameObjs, c => c.Move(secsPassed));
            Parallel.ForEach(GameObjs, c => c.Interact(secsPassed));

            GameObjs.RemoveAll(go => !go.Active);
        }

        public void Draw(SpriteBatch sb) {
            var visRect = Game.Cam.VisibleArea;
            foreach(var c in GameObjs.Where(go => go.DrawRectangle.Intersects(visRect))) {
                c.Draw(sb);
            }
        }

        private void Populate() {
            for(int i = 0; i < CreatureCount; i++) {
                var @new = new Creature(this, Game.Res, GeomHelper.RandomPointInCircel(Game.Rand, WorldSize));
                GameObjs.Add(@new);
            }

            for(int i = 0; i < FoodCount; i++) {
                var @new = new Food(this, Game.Res, GeomHelper.RandomPointInCircel(Game.Rand, WorldSize));
                GameObjs.Add(@new);
            }
        }
    }
}