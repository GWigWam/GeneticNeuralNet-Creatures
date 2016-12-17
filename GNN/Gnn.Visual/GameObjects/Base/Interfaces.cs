using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual.GameObjects.Base {

    public interface IInteracts {

        void Interact(float secsPassed);
    }

    public interface IMoves {

        void Move(float secsPassed);
    }

    public interface IDrawable {

        void Draw(SpriteBatch sb);
    }
}