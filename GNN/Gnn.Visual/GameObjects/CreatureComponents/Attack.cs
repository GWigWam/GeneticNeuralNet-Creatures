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
        internal const float AttackCooldownSec = 2.5f;
        internal const float AttackVisibleSec = 0.25f;
        internal const float AttackHealthCost = 0.4f;
        internal const float AttackDamage = 1.5f;

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

                var hits = Owner.World.CreatureObjs
                    .Where(c => c != Owner && Vector2.Distance(Owner.CenterPosition, c.CenterPosition) <= Radius + c.Radius)
                    .ToArray();
                foreach(var hit in hits) {
                    if(hit.Health > AttackDamage) {
                        hit.Health -= AttackDamage;
                        Owner.Health += AttackDamage;
                    } else {
                        Owner.Health += hit.Health;
                        hit.Health = 0;
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