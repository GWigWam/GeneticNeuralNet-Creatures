using Gnn.Genetic;
using Gnn.NeuralNet;
using Gnn.NeuralNet.Structures.TransferFunctions;
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
        public const int CreatureCount = 100;
        public const int FoodCount = 200;
        public const int ObjCount = CreatureCount + FoodCount;

        public const int WorldSize = 2500;
        public const float MaxWorldTimeSec = 60f;

        private static TransferFunction Transfer = HyperbolicTangentFunction.Instance;

        public GameObject[] AllGameObjs { get; }
        public GameObject[] ActiveGameObjs;

        public float WorldTotalTimeSec { get; private set; }

        private MainGame Game { get; }

        public World(MainGame game) {
            Game = game;
            AllGameObjs = new GameObject[ObjCount];
            ActiveGameObjs = new GameObject[0];
        }

        public void Initialize() {
            InitPopulate();
        }

        public void Update(MouseState mState, KeyboardState kState, Point mPosInWorld, float secsPassed) {
            ActiveGameObjs = AllGameObjs.Where(go => go.Active).ToArray();
            Parallel.ForEach(ActiveGameObjs, c => c.Move(secsPassed));
            Parallel.ForEach(ActiveGameObjs, c => c.Interact(secsPassed));

            WorldTotalTimeSec += secsPassed;
            if(WorldTotalTimeSec > MaxWorldTimeSec || ActiveGameObjs.OfType<Creature>().Count() == 0) {
                RePopulate();
                WorldTotalTimeSec = 0;
            }
        }

        public void Draw(SpriteBatch sb) {
            var visRect = Game.Cam.VisibleArea;
            foreach(var c in ActiveGameObjs.Where(go => go.DrawRectangle.Intersects(visRect))) {
                c.Draw(sb);
            }
        }

        private void InitPopulate() {
            for(int i = 0; i < CreatureCount; i++) {
                var @new = new Creature(this, Game.Res, GeomHelper.RandomPointInCircel(Game.Rand, WorldSize));
                AllGameObjs[i] = @new;
            }

            for(int i = 0; i < FoodCount; i++) {
                var @new = new Food(this, Game.Res, GeomHelper.RandomPointInCircel(Game.Rand, WorldSize));
                AllGameObjs[i + CreatureCount] = @new;
            }
        }

        private void RePopulate() {
            var creatures = AllGameObjs.OfType<Creature>().ToArray();
            var plants = AllGameObjs.OfType<Food>().ToArray();

            var gen = new Genetic.Genetic(0.02f, 0.001f, () => Helpers.MathHelper.Random(Transfer.XMin, Transfer.XMax));
            var indvdl = creatures
                .Select(c => c.Brain.ToIndividual(c.Lifespan));
            var res = gen.Apply(indvdl).ToArray();

            for(int i = 0; i < creatures.Length; i++) {
                var nw = creatures[i].Brain.SetWeights(res[i]);
                var @new = new Creature(this, Game.Res, GeomHelper.RandomPointInCircel(Game.Rand, WorldSize), brain: nw);
                AllGameObjs[i] = @new;
            }

            for(int i = 0; i < FoodCount; i++) {
                var @new = new Food(this, Game.Res, plants[i].CenterPosition);
                AllGameObjs[i + CreatureCount] = @new;
            }
        }
    }
}