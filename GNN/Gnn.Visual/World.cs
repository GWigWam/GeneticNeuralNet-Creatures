using Gnn.Genetic;
using Gnn.NeuralNet;
using Gnn.NeuralNet.Structures.TransferFunctions;
using Gnn.Visual.GameObjects;
using Gnn.Visual.GameObjects.Base;
using Gnn.Visual.GameObjects.CreatureComponents;
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
        public const int CreatureCount = 150;
        public const float FoodToCreatureRatio = 2f;

        public const int WorldSize = 2500;
        public const float MaxWorldTimeSec = 90f;

        private static TransferFunction Transfer = HyperbolicTangentFunction.Instance;

        public List<Food> FoodObjs { get; }
        public Creature[] CreatureObjs { get; private set; }

        public GameObject[] ActiveGameObjs;
        public IEnumerable<GameObject> AllGameObjs => CreatureObjs.Cast<GameObject>().Concat(FoodObjs);

        public float WorldTotalTimeSec { get; private set; }
        public int GenerationCount { get; private set; }

        private string StatusStr = string.Empty;
        public MainGame Game { get; }

        public World(MainGame game) {
            Game = game;

            CreatureObjs = new Creature[CreatureCount];
            FoodObjs = new List<Food>();
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

            FoodObjs.RemoveAll(f => !f.Active);
            var shortage = (ActiveGameObjs.OfType<Creature>().Count() * FoodToCreatureRatio) - FoodObjs.Count;
            for(int i = 0; i < shortage; i++) {
                var @new = new Food(this, Game.Res, GeomHelper.RandomPointInCircel(Game.Rand, WorldSize));
                FoodObjs.Add(@new);
            }
        }

        public void DrawRelative(SpriteBatch sb) {
            var visRect = Game.Cam.VisibleArea;
            foreach(var c in ActiveGameObjs.Where(go => go.DrawRectangle.Intersects(visRect))) {
                c.Draw(sb);
            }
        }

        public void DrawStatic(SpriteBatch sb) {
            sb.DrawString(Game.Res.FConsolas, StatusStr, new Vector2(0, 30), Color.Black);
        }

        private void InitPopulate() {
            for(int i = 0; i < CreatureCount; i++) {
                var @new = new Creature(this, Game.Res, GeomHelper.RandomPointInCircel(Game.Rand, WorldSize));
                CreatureObjs[i] = @new;
            }
        }

        private void RePopulate() {
            GenerationCount++;
            FoodObjs.Clear();
            var creatures = AllGameObjs.OfType<Creature>().ToArray();

            var gen = new Genetic.Genetic(0.04f, 0.001f, () => Helpers.MathHelper.Random(Transfer.XMin, Transfer.XMax));
            var minLifeSpan = creatures.Min(c => c.Lifespan);
            var indvdl = creatures
                .Select(c => c.Brain.Net.ToIndividual((c.Lifespan - minLifeSpan) + (c.Health / Creature.IdleHealthLossPerSecond / 2f)));
            var res = gen.Apply(indvdl).ToArray();

            for(int i = 0; i < creatures.Length; i++) {
                var nw = creatures[i].Brain.Net.SetWeights(res[i]);
                var @new = new Creature(this, Game.Res, GeomHelper.RandomPointInCircel(Game.Rand, WorldSize), initNw: nw);
                CreatureObjs[i] = @new;
            }

            StatusStr = $"LF: {creatures.Min(c => c.Lifespan)} : {creatures.Max(c => c.Lifespan)} : {creatures.Average(c => c.Lifespan)}\nVar={indvdl.AvgVariety()} Gen#={GenerationCount}";
        }
    }
}