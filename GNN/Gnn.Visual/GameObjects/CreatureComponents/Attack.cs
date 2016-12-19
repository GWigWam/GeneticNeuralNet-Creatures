using Gnn.Visual.GameObjects.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual.GameObjects.CreatureComponents {

    internal class Attack : Drawable, IInteracts {
        internal const float AttackCooldownSec = 3.5f;
        internal const float AttackVisibleSec = 0.2f;
        internal const float AttackHealthCost = 1f;

        private Creature Owner;
        private MainGameContent Res;

        public override int DrawX => (int)Math.Round(Owner.CenterPosition.X);
        public override int DrawY => (int)Math.Round(Owner.CenterPosition.Y);

        public float TimeSinceLastAttack { get; private set; } = float.MaxValue;
        public int Radius { get; }

        public Attack(Creature owner, MainGameContent res) : base(res.TAttack, DrawableOrigin.Center) {
            Owner = owner;
            Res = res;
            Radius = Res.TAttack.Width / 2;
        }

        public void DoAttack() {
            if(TimeSinceLastAttack >= AttackCooldownSec && Owner.Health > AttackHealthCost) {
                TimeSinceLastAttack = 0;
                Owner.Health -= AttackHealthCost;

                foreach(var hit in Owner.World.CreatureObjs.Where(c => c != Owner && Vector2.Distance(Owner.CenterPosition, c.CenterPosition) <= Radius + c.Radius)) {
                    while(hit.Health > 0) {
                        var hp = hit.Health > Food.MaxHealth ? Food.MaxHealth : hit.Health;
                        hit.Health -= hp;

                        var pos = GeomHelper.RandomPointInCircel(ThreadSafeRandom.Instance, hit.Radius) + hit.CenterPosition;
                        var newFood = new Food(Owner.World, Res, pos) {
                            Health = hp
                        };
                        Owner.World.FoodObjs.Add(newFood);
                    }
                }
            }
        }

        public void Interact(float secsPassed) {
            TimeSinceLastAttack += secsPassed;

            if(Owner.Brain.ShouldAttack) {
                DoAttack();
            }
        }

        public override void Draw(SpriteBatch sb) {
            if(TimeSinceLastAttack <= AttackVisibleSec) {
                base.Draw(sb);
            }
        }
    }
}