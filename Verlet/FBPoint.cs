using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookCollision.Verlet
{
    class FBVerletPoint
    {
        public Vector2 Position;
        public Vector2 OldPosition;
        public Vector2 Acceleration;
    }
}
